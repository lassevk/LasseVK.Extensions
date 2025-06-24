using System.Reflection;

using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;
using LasseVK.Extensions.Hosting.ConsoleApplications.GlobalCommands;

using Microsoft.Extensions.DependencyInjection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

internal class ConsoleApplicationHostedServiceOptions
{
    private Func<IServiceProvider, IConsoleApplication>? _consoleApplicationFactory;
    private Dictionary<string, Func<IServiceProvider, IConsoleApplication>> _commands = new(StringComparer.InvariantCultureIgnoreCase);

    public ConsoleApplicationHostedServiceOptions SetConsoleApplication<T>(Action<T>? configure)
        where T : IConsoleApplication
    {
        if (_consoleApplicationFactory != null)
        {
            throw new InvalidOperationException("Only one console application can be configured.");
        }

        _consoleApplicationFactory = services =>
        {
            T consoleApplication = ActivatorUtilities.CreateInstance<T>(services);
            configure?.Invoke(consoleApplication);
            return consoleApplication;
        };

        return this;
    }

    public IConsoleApplication? GetConsoleApplication(IServiceProvider services)
    {
        if (_consoleApplicationFactory != null)
        {
            return _consoleApplicationFactory?.Invoke(services);
        }

        if (_commands.Count == 0)
        {
            throw new InvalidOperationException("No console application configured.");
        }

        ConsoleCommandDispatcher application = ActivatorUtilities.CreateInstance<ConsoleCommandDispatcher>(services);
        application.AddCommands(new Dictionary<string, Func<IServiceProvider, IConsoleApplication>>
        {
            ["help"] = srv => ActivatorUtilities.CreateInstance<HelpCommand>(srv),
        });
        application.AddCommands(_commands);
        return application;
    }

    public void AddCommand<T>(Action<T>? configure)
        where T : class, IConsoleApplication
    {
        string name = typeof(T).GetCustomAttribute<ConsoleCommandAttribute>()?.Name ?? typeof(T).Name.Replace("Command", "");;

        _commands.Add(name, services =>
        {
            T command = ActivatorUtilities.CreateInstance<T>(services);
            configure?.Invoke(command);
            return command;
        });
    }

    public void AddCommand(Type type)
    {
        string name = type.GetCustomAttribute<ConsoleCommandAttribute>()?.Name ?? type.Name.Replace("Command", "");;

        _commands.Add(name, services => (IConsoleApplication)ActivatorUtilities.CreateInstance(services, type));
    }
}
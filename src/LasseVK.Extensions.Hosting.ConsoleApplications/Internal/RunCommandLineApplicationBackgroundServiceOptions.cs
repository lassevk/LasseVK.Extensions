using System.Reflection;

using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;
using LasseVK.Extensions.Hosting.ConsoleApplications.GlobalCommands;

using Microsoft.Extensions.DependencyInjection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

internal class RunCommandLineApplicationBackgroundServiceOptions
{
    private Func<IServiceProvider, ICommandLineApplication>? _consoleApplicationFactory;
    private Dictionary<string, Func<IServiceProvider, ICommandLineApplication>> _commands = new(StringComparer.InvariantCultureIgnoreCase);

    public RunCommandLineApplicationBackgroundServiceOptions SetConsoleApplication<T>(Action<T>? configure)
        where T : ICommandLineApplication
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

    public ICommandLineApplication? GetConsoleApplication(IServiceProvider services)
    {
        if (_consoleApplicationFactory != null)
        {
            return _consoleApplicationFactory?.Invoke(services);
        }

        if (_commands.Count == 0)
        {
            throw new InvalidOperationException("No console application configured.");
        }

        RunCommandLineCommandConsoleApplication application = ActivatorUtilities.CreateInstance<RunCommandLineCommandConsoleApplication>(services);
        application.AddCommands(new Dictionary<string, Func<IServiceProvider, ICommandLineApplication>>
        {
            ["help"] = srv => ActivatorUtilities.CreateInstance<HelpCommand>(srv),
        });
        application.AddCommands(_commands);
        return application;
    }

    public void AddCommand<T>(Action<T>? configure)
        where T : class, ICommandLineApplication
    {
        string name = typeof(T).GetCustomAttribute<CommandLineCommandAttribute>()?.Name ?? typeof(T).Name.Replace("Command", "");;

        _commands.Add(name, services =>
        {
            T command = ActivatorUtilities.CreateInstance<T>(services);
            configure?.Invoke(command);
            return command;
        });
    }
}
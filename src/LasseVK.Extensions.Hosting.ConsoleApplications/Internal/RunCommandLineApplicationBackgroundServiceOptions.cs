using Microsoft.Extensions.DependencyInjection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

public class RunCommandLineApplicationBackgroundServiceOptions
{
    private Func<IServiceProvider, ICommandLineApplication>? _consoleApplicationFactory;

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

    public ICommandLineApplication? GetConsoleApplication(IServiceProvider services) => _consoleApplicationFactory?.Invoke(services);
}
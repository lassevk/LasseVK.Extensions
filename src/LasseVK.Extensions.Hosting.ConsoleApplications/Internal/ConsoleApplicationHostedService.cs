﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

internal class ConsoleApplicationHostedService : BackgroundService
{
    private readonly IOptions<ConsoleApplicationHostedServiceOptions> _options;
    private readonly IServiceProvider _services;
    private readonly ILogger<ConsoleApplicationHostedService> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public ConsoleApplicationHostedService(IOptions<ConsoleApplicationHostedServiceOptions> options, IServiceProvider services, ILogger<ConsoleApplicationHostedService> logger,
        IHostApplicationLifetime  hostApplicationLifetime)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IConsoleApplication? consoleApplication = _options.Value.GetConsoleApplication(_services);

        if (consoleApplication is null)
        {
            throw new InvalidOperationException("No console application configured.");
        }

        try
        {
            CommandLineArgumentsInjector.Inject(Environment.GetCommandLineArgs().Skip(1).ToArray(), consoleApplication);

            var tcs = new TaskCompletionSource<bool>();
            _hostApplicationLifetime.ApplicationStarted.Register(() => tcs.TrySetResult(true));
            _hostApplicationLifetime.ApplicationStopping.Register(() => tcs.TrySetCanceled(_hostApplicationLifetime.ApplicationStopping));
            await tcs.Task;

            _logger.LogDebug("Starting console application");
            Environment.ExitCode = await consoleApplication.RunAsync(stoppingToken);
            _logger.LogDebug("Console application terminated normally, with an exit code of {ExitCode}", Environment.ExitCode);
        }
        catch (TaskCanceledException)
        {
            _logger.LogDebug("Console application terminated by user");
            Environment.ExitCode = 1;
        }
        catch (CommandLineArgumentsParsingException ex)
        {
            await Console.Error.WriteLineAsync("error: " + ex.Message);
            Environment.ExitCode = 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Console application terminated unexpectedly");
            await Console.Error.WriteLineAsync(ex.Message);
            Environment.ExitCode = 1;
        }
        finally
        {
            _hostApplicationLifetime.StopApplication();
        }
    }
}
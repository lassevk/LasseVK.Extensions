using System.Reflection;

using LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LasseVK.Extensions.Hosting.ConsoleApplications;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddCommandLineApplication<T>(this IHostApplicationBuilder builder, Action<T>? configure = null)
        where T : class, ICommandLineApplication
    {
        builder.Services.AddHostedService<RunCommandLineApplicationBackgroundService>();
        builder.Services.Configure<RunCommandLineApplicationBackgroundServiceOptions>(options => options.SetConsoleApplication(configure));
        return builder;
    }

    public static IHostApplicationBuilder AddCommandLineCommand<T>(this IHostApplicationBuilder builder, Action<T>? configure = null)
        where T : class, ICommandLineApplication
    {
        builder.Services.AddHostedService<RunCommandLineApplicationBackgroundService>();
        builder.Services.Configure<RunCommandLineApplicationBackgroundServiceOptions>(options => options.AddCommand(configure));
        return builder;
    }

    public static IHostApplicationBuilder AddCommandLineCommands<TProgram>(this IHostApplicationBuilder builder)
        where TProgram : class
    {
        builder.Services.AddHostedService<RunCommandLineApplicationBackgroundService>();

        foreach (Type type in typeof(TProgram).Assembly.GetTypes())
        {
            if (type.IsAssignableTo(typeof(ICommandLineApplication)) && !type.IsAbstract)
            {
                builder.Services.Configure<RunCommandLineApplicationBackgroundServiceOptions>(options => options.AddCommand(type));
            }
        }

        return builder;
    }
}
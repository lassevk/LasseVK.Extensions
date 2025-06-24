using System.Reflection;

using LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LasseVK.Extensions.Hosting.ConsoleApplications;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddConsoleApplication<T>(this IHostApplicationBuilder builder, Action<T>? configure = null)
        where T : class, IConsoleApplication
    {
        builder.Services.AddHostedService<ConsoleApplicationHostedService>();
        builder.Services.Configure<ConsoleApplicationHostedServiceOptions>(options => options.SetConsoleApplication(configure));
        return builder;
    }

    public static IHostApplicationBuilder AddConsoleCommand<T>(this IHostApplicationBuilder builder, Action<T>? configure = null)
        where T : class, IConsoleApplication
    {
        builder.Services.AddHostedService<ConsoleApplicationHostedService>();
        builder.Services.Configure<ConsoleApplicationHostedServiceOptions>(options => options.AddCommand(configure));
        return builder;
    }

    public static IHostApplicationBuilder AddConsoleCommands<TProgram>(this IHostApplicationBuilder builder)
        where TProgram : class
    {
        builder.Services.AddHostedService<ConsoleApplicationHostedService>();

        foreach (Type type in typeof(TProgram).Assembly.GetTypes())
        {
            if (type.IsAssignableTo(typeof(IConsoleApplication)) && !type.IsAbstract)
            {
                builder.Services.Configure<ConsoleApplicationHostedServiceOptions>(options => options.AddCommand(type));
            }
        }

        return builder;
    }
}
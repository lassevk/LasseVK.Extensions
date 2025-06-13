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
}
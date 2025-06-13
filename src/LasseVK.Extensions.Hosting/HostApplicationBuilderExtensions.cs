using LasseVK.Extensions.Configuration;

using Microsoft.Extensions.Hosting;

namespace LasseVK.Extensions.Hosting;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder RelocateConfigurationFiles<TProgram>(this IHostApplicationBuilder builder)
        where TProgram : class
    {
        builder.Configuration.RelocateConfigurationFiles<TProgram>(builder.Environment.EnvironmentName);
        return builder;
    }
}
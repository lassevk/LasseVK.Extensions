using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace LasseVK.Extensions.Configuration;

public static class ConfigurationManagerExtensions
{
    public static IConfigurationManager RelocateConfigurationFiles<TProgram>(this IConfigurationManager configuration, string environmentName)
        where TProgram : class
    {
        ArgumentNullException.ThrowIfNull(configuration);

        IConfigurationSource[] beforeJson = configuration.Sources.TakeWhile(source => source is not JsonConfigurationSource).ToArray();
        IConfigurationSource[] afterJson = configuration.Sources.SkipWhile(source => source is not JsonConfigurationSource).SkipWhile(source => source is JsonConfigurationSource).ToArray();
        configuration.Sources.Clear();
        foreach (IConfigurationSource source in beforeJson)
        {
            configuration.Sources.Add(source);
        }

        configuration.SetBasePath(Path.GetDirectoryName(typeof(TProgram).Assembly.Location)!);
        configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        configuration.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
        configuration.AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true);
        configuration.AddJsonFile($"appsettings.{Environment.MachineName}.{environmentName}.json", optional: true, reloadOnChange: true);

        foreach (IConfigurationSource source in afterJson)
        {
            configuration.Sources.Add(source);
        }

        return configuration;
    }
}
using System.Reflection;
using System.Text;

using LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;
using LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

namespace LasseVK.Extensions.Hosting.ConsoleApplications;

public static class CommandLineHelp
{
    public static IEnumerable<string> GetHelp(object instance, string? applicationName = null)
    {
        ArgumentNullException.ThrowIfNull(instance);

        applicationName = applicationName ?? Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException("Unable to determine application name.");

        // TODO: internal commands
        return GenerateHelpText(instance, applicationName);
    }

    private static IEnumerable<string> GenerateHelpText(object instance, string title)
    {
        Dictionary<string, ICommandLineProperty> optionProperties = CommandLineArgumentsReflector.ReflectOptionProperties(instance);
        List<PositionalArgumentHandler> positionalProperties = CommandLineArgumentsReflector.ReflectPositionalProperties(instance);

        var first = new StringBuilder(title);
        foreach (PositionalArgumentHandler positionalProperty in positionalProperties.Where(positionalProperty
            => positionalProperty.Position < int.MaxValue || positionalProperty.Property is not StringListCommandLineProperty))
        {
            first.Append(" <");
            first.Append(positionalProperty.Property.Name);
            first.Append('>');
        }

        if (optionProperties.Count > 0)
        {
            first.Append(" [options]");
        }

        PositionalArgumentHandler? restProperty = positionalProperties.FirstOrDefault(prop => prop is { Position: int.MaxValue, Property: StringListCommandLineProperty });
        if (restProperty is not null)
        {
            first.Append(" [");
            first.Append(restProperty.Property.Name);
            first.Append(" ...]");
        }

        yield return first.ToString();

        var properties = optionProperties.GroupBy(kvp => kvp.Value).OrderBy(g => g.First().Key, StringComparer.InvariantCultureIgnoreCase).ToList();
        foreach (IGrouping<ICommandLineProperty, KeyValuePair<string, ICommandLineProperty>> property in properties)
        {
            yield return "";
            yield return "  " + string.Join(", ", property.OrderBy(kvp => kvp.Key.Length).Select(kvp => "-" + kvp.Key + " " + kvp.Value.Name));

            foreach (string helpText in property.First().Value.GetHelpText())
            {
                yield return "    " + helpText;
            }
        }

        foreach (PositionalArgumentHandler positionalProperty in positionalProperties)
        {
            yield return "";
            yield return "  " + positionalProperty.Property.Name;

            foreach (string helpText in positionalProperty.Property.GetHelpText())
            {
                yield return "    " + helpText;
            }
        }
    }
}
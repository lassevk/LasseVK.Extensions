using System.Reflection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal class BooleanCommandLineProperty : CommandLineProperty, ICommandLineProperty
{
    private readonly PropertyInfo _property;
    private readonly object _instance;

    private bool _valueWasSet;

    private BooleanCommandLineProperty(PropertyInfo property, object instance, string name, string? description)
        : base(name, description)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public ArgumentHandlerAcceptResponse Accept(string argument)
    {
        bool? value = argument.ToLowerInvariant() switch
        {
            "yes"  => true,
            "1"    => true,
            "on"   => true,
            "true" => true,

            "no"    => false,
            "0"     => false,
            "off"   => false,
            "false" => false,

            _ => null,
        };

        if (value is null)
        {
            return ArgumentHandlerAcceptResponse.InvalidValue;
        }

        _property.SetValue(_instance, value);
        _valueWasSet = true;
        return ArgumentHandlerAcceptResponse.Finished;
    }

    public ArgumentHandlerFinishResponse Finish()
    {
        if (!_valueWasSet)
        {
            _property.SetValue(_instance, true);
        }

        return ArgumentHandlerFinishResponse.Finished;
    }

    public override IEnumerable<string> GetHelpText()
    {
        foreach (string line in base.GetHelpText())
        {
            yield return line;
        }

        yield return Name + " is a boolean value, valid values are: yes, 1, on, true, no, 0, off, false";
        yield return "if the value is omitted, the value is assumed to be true";
    }

    public static ICommandLineProperty Factory(PropertyInfo property, object instance, string name, string? description) => new BooleanCommandLineProperty(property, instance, name, description);
}
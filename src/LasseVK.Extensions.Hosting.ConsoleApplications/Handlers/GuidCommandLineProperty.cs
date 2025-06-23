using System.Reflection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal class GuidCommandLineProperty : CommandLineProperty, ICommandLineProperty
{
    private readonly PropertyInfo _property;
    private readonly object _instance;

    private bool _valueWasSet;

    private GuidCommandLineProperty(PropertyInfo property, object instance, string name, string? description)
        : base(name, description)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public ArgumentHandlerAcceptResponse Accept(string argument)
    {
        if (!Guid.TryParse(argument, out Guid value))
        {
            return ArgumentHandlerAcceptResponse.InvalidValue;
        }

        _property.SetValue(_instance, value);
        _valueWasSet = true;
        return ArgumentHandlerAcceptResponse.Finished;
    }

    public ArgumentHandlerFinishResponse Finish() => !_valueWasSet ? ArgumentHandlerFinishResponse.MissingValue : ArgumentHandlerFinishResponse.Finished;

    public override IEnumerable<string> GetHelpText()
    {
        foreach (string line in base.GetHelpText())
        {
            yield return line;
        }

        yield return Name + " is a GUID, format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX where each X is a hexadecimal digit, 0-9 or A-F.";
        yield return "Accepts both uppercase and lowercase letters.";
    }

    public static ICommandLineProperty Factory(PropertyInfo property, object instance, string name, string? description) => new GuidCommandLineProperty(property, instance, name, description);
}
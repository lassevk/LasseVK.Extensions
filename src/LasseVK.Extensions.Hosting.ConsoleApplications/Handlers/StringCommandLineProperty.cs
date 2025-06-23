using System.Reflection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal class StringCommandLineProperty : CommandLineProperty, ICommandLineProperty
{
    private readonly PropertyInfo _property;
    private readonly object _instance;

    private bool _valueWasSet;

    private StringCommandLineProperty(PropertyInfo property, object instance, string name, string? description)
        : base(name, description)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public ArgumentHandlerAcceptResponse Accept(string argument)
    {
        _property.SetValue(_instance, argument);
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

        yield return Name + " is a string, see help text for purpose which should indicate acceptable values and formats";
    }

    public static ICommandLineProperty Factory(PropertyInfo property, object instance, string name, string? description) => new StringCommandLineProperty(property, instance, name, description);
}
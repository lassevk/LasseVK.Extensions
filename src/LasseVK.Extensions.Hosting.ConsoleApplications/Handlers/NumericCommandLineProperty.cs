using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal class NumericCommandLineProperty<T> : CommandLineProperty, ICommandLineProperty
    where T : INumber<T>
{
    private readonly PropertyInfo _property;
    private readonly object _instance;

    private bool _valueWasSet;

    private NumericCommandLineProperty(PropertyInfo property, object instance, string name, string? description)
        : base(name, description)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public ArgumentHandlerAcceptResponse Accept(string argument)
    {
        argument = argument.Replace("_", "");
        argument = argument.Replace(",", ".");

        if (!T.TryParse(argument, CultureInfo.InvariantCulture, out T? value))
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

        yield return Name + " is a number of type " + typeof(T).Name;
    }

    public static ICommandLineProperty Factory(PropertyInfo property, object instance, string name, string? description)
        => new NumericCommandLineProperty<T>(property, instance, name, description);
}
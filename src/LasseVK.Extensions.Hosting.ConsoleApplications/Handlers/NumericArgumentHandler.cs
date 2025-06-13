using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal class NumericArgumentHandler<T> : IArgumentHandler
    where T : INumber<T>
{
    private readonly PropertyInfo _property;
    private readonly object _instance;

    private bool _valueWasSet;

    private NumericArgumentHandler(PropertyInfo property, object instance, string name)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        Name = name;
    }

    public string Name { get; }

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

    public static IArgumentHandler Factory(PropertyInfo property, object instance, string name)
        => new NumericArgumentHandler<T>(property, instance, name);
}
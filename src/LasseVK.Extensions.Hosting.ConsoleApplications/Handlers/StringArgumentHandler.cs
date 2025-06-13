using System.Reflection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal class StringArgumentHandler : IArgumentHandler
{
    private readonly PropertyInfo _property;
    private readonly object _instance;

    private bool _valueWasSet;

    private StringArgumentHandler(PropertyInfo property, object instance, string name)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        Name = name;
    }

    public string Name { get; }

    public ArgumentHandlerAcceptResponse Accept(string argument)
    {
        _property.SetValue(_instance, argument);
        _valueWasSet = true;
        return ArgumentHandlerAcceptResponse.Finished;
    }

    public ArgumentHandlerFinishResponse Finish() => !_valueWasSet ? ArgumentHandlerFinishResponse.MissingValue : ArgumentHandlerFinishResponse.Finished;

    public static IArgumentHandler Factory(PropertyInfo property, object instance, string name) => new StringArgumentHandler(property, instance, name);
}
using System.Reflection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal class StringListArgumentHandler : IArgumentHandler
{
    private readonly List<string> _list;

    private StringListArgumentHandler(List<string> list, string name)
    {
        _list = list;
        Name = name;
    }

    public string Name { get; }

    public ArgumentHandlerAcceptResponse Accept(string argument)
    {
        _list.Add(argument);
        return ArgumentHandlerAcceptResponse.ContinueAccepting;
    }

    public ArgumentHandlerFinishResponse Finish() => ArgumentHandlerFinishResponse.Finished;

    public static IArgumentHandler? Factory(PropertyInfo property, object instance, string name)
    {
        if (property.GetValue(instance) is List<string> list)
        {
            return new StringListArgumentHandler(list, name);
        }

        if (property.CanWrite)
        {
            list = [];
            property.SetValue(instance, list);
            return new StringListArgumentHandler(list, name);
        }

        return null;
    }
}
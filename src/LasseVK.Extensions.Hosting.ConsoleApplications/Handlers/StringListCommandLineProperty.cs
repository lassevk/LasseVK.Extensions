using System.Reflection;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal class StringListCommandLineProperty : CommandLineProperty, ICommandLineProperty
{
    private readonly List<string> _list;

    private StringListCommandLineProperty(List<string> list, string name, string? description)
        : base(name, description)
    {
        _list = list;
    }

    public ArgumentHandlerAcceptResponse Accept(string argument)
    {
        _list.Add(argument);
        return ArgumentHandlerAcceptResponse.ContinueAccepting;
    }

    public ArgumentHandlerFinishResponse Finish() => ArgumentHandlerFinishResponse.Finished;

    public override IEnumerable<string> GetHelpText()
    {
        foreach (string line in base.GetHelpText())
        {
            yield return line;
        }

        yield return Name + " is one or more strings, see help text for purpose which should indicate acceptable values and formats";
        yield return "note that strings can be added one by one, until either the next option (starting with -), or the end of the arguments";
        yield return "if the value is omitted, the list will be empty, no defaults are assumed";
    }

    public static ICommandLineProperty? Factory(PropertyInfo property, object instance, string name, string? description)
    {
        if (property.GetValue(instance) is List<string> list)
        {
            return new StringListCommandLineProperty(list, name, description);
        }

        if (property.CanWrite)
        {
            list = [];
            property.SetValue(instance, list);
            return new StringListCommandLineProperty(list, name, description);
        }

        return null;
    }
}
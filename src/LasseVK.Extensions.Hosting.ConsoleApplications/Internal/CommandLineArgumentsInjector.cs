using LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

public class CommandLineArgumentsInjector
{
    public static void Inject(string[] args, object instance)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(instance);

        Dictionary<string, IArgumentHandler> handlers = GetArgumentHandlers(instance);
        List<IArgumentHandler> positionalHandlers = GetPositionalArgumentHandlers(instance);

        var context = new CommandLineArgumentsContext(handlers, positionalHandlers);
        foreach (string argument in args)
        {
            context.Process(argument);
        }

        context.Finish();

        if (context.Errors.Count > 0)
        {
            throw new CommandLineArgumentsParsingException(context.Errors);
        }
    }

    private static Dictionary<string, IArgumentHandler> GetArgumentHandlers(object instance)
    {
        Dictionary<string, IArgumentHandler> handlers = [];

        Dictionary<string, IArgumentHandler> handlersForInstance = CommandLineArgumentsReflector.ReflectOptionProperties(instance);
        foreach (KeyValuePair<string, IArgumentHandler> kvp in handlersForInstance)
        {
            handlers.Add(kvp.Key, kvp.Value);
        }

        return handlers;
    }

    private static List<IArgumentHandler> GetPositionalArgumentHandlers(object instance)
    {
        List<PositionalArgumentHandler> positionalHandlers = CommandLineArgumentsReflector.ReflectPositionalProperties(instance);

        positionalHandlers.Sort((x, y) => x.Position.CompareTo(y.Position));

        return positionalHandlers.Select(pah => pah.Handler).ToList();
    }
}
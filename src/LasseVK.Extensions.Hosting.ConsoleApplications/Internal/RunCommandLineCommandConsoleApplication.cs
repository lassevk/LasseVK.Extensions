using System.ComponentModel;

using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;
using LasseVK.Extensions.Hosting.ConsoleApplications.GlobalCommands;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

internal class RunCommandLineCommandConsoleApplication : ICommandLineApplication
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, Func<IServiceProvider, ICommandLineApplication>> _commands = new(StringComparer.InvariantCultureIgnoreCase);

    public RunCommandLineCommandConsoleApplication(IServiceProvider services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    [PositionalArgument(0)]
    [ArgumentName("COMMAND")]
    [StopParsingOptionsAfter]
    [Description("The command to run, use HELP for a list of commands or HELP <command> for help on a specific command")]
    public string CommandName { get; set; } = "";

    [RestArguments]
    [ArgumentName("ARGS")]
    [Description("Arguments to pass to the command")]
    public List<string> ArgumentsToCommand { get; } = [];

    public void AddCommands(Dictionary<string, Func<IServiceProvider, ICommandLineApplication>> commands)
    {
        foreach (KeyValuePair<string, Func<IServiceProvider, ICommandLineApplication>> kvp in commands)
        {
            _commands.Add(kvp.Key, kvp.Value);
        }
    }

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (string.IsNullOrWhiteSpace(CommandName))
        {
            ShowGlobalHelp();
            return 1;
        }

        if (!_commands.TryGetValue(CommandName, out Func<IServiceProvider, ICommandLineApplication>? commandFactory))
        {
            await Console.Error.WriteLineAsync($"error: unknown command {CommandName}");
            return 1;
        }

        ICommandLineApplication command = commandFactory(_services);
        CommandLineArgumentsInjector.Inject(ArgumentsToCommand.ToArray(), command);
        if (command is HelpCommand help)
        {
            help.AddCommands(_commands);
        }

        return await command.RunAsync(cancellationToken);
    }

    private void ShowGlobalHelp()
    {
        foreach (string line in CommandLineHelp.GetApplicationHelp(this))
        {
            Console.WriteLine(line);
        }
    }
}
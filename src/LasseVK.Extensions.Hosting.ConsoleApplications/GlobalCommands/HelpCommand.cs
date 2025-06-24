using System.ComponentModel;

using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.GlobalCommands;

[ConsoleCommand("help")]
[Description("Shows help for the application or for a specific command.\nUse 'help' to show a list of available commands.\nUse 'help <command>' to show help for a specific command.")]
internal class HelpCommand : IConsoleApplication
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, Func<IServiceProvider, IConsoleApplication>> _commands = new(StringComparer.InvariantCultureIgnoreCase);

    public HelpCommand(IServiceProvider services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    [PositionalArgument(0)]
    [ArgumentName("COMMAND")]
    [Description("The command to show help for")]
    public string CommandName { get; set; } = "";

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (string.IsNullOrWhiteSpace(CommandName))
        {
            ShowListOfCommands();
            return 0;
        }

        if (!_commands.TryGetValue(CommandName, out Func<IServiceProvider, IConsoleApplication>? commandFactory))
        {
            await Console.Error.WriteLineAsync($"error: unknown command {CommandName}");
            return 1;
        }

        IConsoleApplication command = commandFactory(_services);
        foreach (string line in CommandLineHelp.GetCommandHelp(command, CommandName.ToUpperInvariant()))
        {
            Console.WriteLine(line);
        }

        return 0;
    }

    private void ShowListOfCommands()
    {
        Console.WriteLine("Available commands:");

        foreach (string key in _commands.Keys.OrderBy(x => x))
        {
            Console.WriteLine($"  {key}");
        }

        Console.WriteLine();
        Console.WriteLine("Use 'help <command>' for more information on a specific command.");
    }

    public void AddCommands(Dictionary<string, Func<IServiceProvider, IConsoleApplication>> commands)
    {
        foreach (KeyValuePair<string, Func<IServiceProvider, IConsoleApplication>> kvp in commands)
        {
            _commands.Add(kvp.Key, kvp.Value);
        }
    }
}
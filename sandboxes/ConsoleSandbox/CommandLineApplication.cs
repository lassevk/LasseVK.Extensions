using System.ComponentModel;

using LasseVK.Extensions.Hosting.ConsoleApplications;
using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

public class CommandLineApplication : ICommandLineApplication
{
    [Option("v")]
    [Option("verbose")]
    [ArgumentName("VERBOSITY")]
    [Description("set verbosity level")]
    public bool Verbose { get; set; }

    [RestArguments]
    [ArgumentName("FILES")]
    [Description("one or more files to process, must include full path to file")]
    public List<string> Filenames { get; } = [];

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        foreach (string line in CommandLineHelp.GetHelp(this))
        {
            Console.WriteLine(line);
        }

        return 0;
    }
}
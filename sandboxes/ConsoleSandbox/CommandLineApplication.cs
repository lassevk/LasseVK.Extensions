using System.ComponentModel;

using LasseVK.Extensions.Hosting.ConsoleApplications;
using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

namespace ConsoleSandbox;

public class CommandLineApplication : ICommandLineApplication
{
    [Option("v")]
    [Option("verbose")]
    [ArgumentName("VERBOSITY")]
    [Description("set verbosity level")]
    public bool Verbose { get; set; }

    [Option("h")]
    [Option("help")]
    [ArgumentName("HELP")]
    [Description("show help for the application")]
    public bool ShowHelp { get; set; }

    [RestArguments]
    [ArgumentName("FILES")]
    [Description("one or more files to process, must include full path to file")]
    public List<string> Filenames { get; } = [];

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (ShowHelp)
        {
            foreach (string line in CommandLineHelp.GetHelp(this))
            {
                Console.WriteLine(line);
            }

            return 0;
        }

        if (Filenames.Count == 0)
        {
            await Console.Error.WriteLineAsync("No files specified");
            return 1;
        }

        foreach (string filename in Filenames)
        {
            Console.WriteLine("Processing " + filename);
        }

        return 0;
    }
}
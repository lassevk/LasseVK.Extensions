using System.ComponentModel;

using LasseVK.Extensions.Hosting.ConsoleApplications;
using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

namespace ConsoleSandbox.Commands;

[ConsoleCommand("list")]
[Description("Lists all files")]
public class ListCommand : IConsoleApplication
{
    [Option("v")]
    [Option("verbose")]
    [Description("Verbose output")]
    [ArgumentName("VERBOSITY")]
    public bool Verbose { get; set; }

    public Task<int> RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Listing");
        return Task.FromResult(0);
    }
}
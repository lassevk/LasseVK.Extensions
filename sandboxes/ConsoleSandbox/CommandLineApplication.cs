using LasseVK.Extensions.Hosting.ConsoleApplications;
using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

public class CommandLineApplication : ICommandLineApplication
{
    [Option("v")]
    [Option("verbose")]
    public bool Verbose { get; set; }

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        Console.WriteLine("Hello World!");
        if (Verbose)
        {
            Console.WriteLine("Verbose");
        }

        return 0;
    }
}
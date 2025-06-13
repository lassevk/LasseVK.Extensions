using LasseVK.Extensions.Hosting.ConsoleApplications;

public class CommandLineApplication : ICommandLineApplication
{
    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Hello World!");
        await Task.Delay(2000, cancellationToken);

        return 0;
    }
}
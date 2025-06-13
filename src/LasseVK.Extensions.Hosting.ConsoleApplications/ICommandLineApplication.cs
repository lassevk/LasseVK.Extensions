namespace LasseVK.Extensions.Hosting.ConsoleApplications;

public interface ICommandLineApplication
{
    Task<int> RunAsync(CancellationToken cancellationToken);
}
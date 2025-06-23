namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal abstract class CommandLineProperty
{
    private readonly string? _description;

    protected CommandLineProperty(string name, string? description)
    {
        _description = description;
        Name = name;
    }

    public string Name { get; }

    public virtual IEnumerable<string> GetHelpText()
    {
        if (_description == null)
        {
            yield break;
        }

        using var reader = new StringReader(_description);
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }

        yield return "";
    }
}
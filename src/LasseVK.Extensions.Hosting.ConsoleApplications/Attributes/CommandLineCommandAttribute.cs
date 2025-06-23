namespace LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandLineCommandAttribute : Attribute
{
    public CommandLineCommandAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
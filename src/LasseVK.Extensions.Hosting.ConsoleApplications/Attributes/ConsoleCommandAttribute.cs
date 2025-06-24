namespace LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ConsoleCommandAttribute : Attribute
{
    public ConsoleCommandAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
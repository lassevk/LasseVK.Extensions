namespace LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ArgumentNameAttribute : Attribute
{
    public ArgumentNameAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public string Name { get; }
}
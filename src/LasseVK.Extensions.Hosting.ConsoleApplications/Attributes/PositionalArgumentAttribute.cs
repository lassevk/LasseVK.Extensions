namespace LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PositionalArgumentAttribute : Attribute
{
    public PositionalArgumentAttribute(int position)
    {
        Position = position;
    }

    public int Position { get; }
}
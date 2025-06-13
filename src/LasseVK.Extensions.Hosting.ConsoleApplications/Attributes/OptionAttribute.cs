namespace LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class OptionAttribute : Attribute
{
    public OptionAttribute(string option)
    {
        if (option.StartsWith('-') || option.StartsWith('/'))
        {
            throw new ArgumentException("Option attributes should exclude the leading - or / to denote a command line option", nameof(option));
        }

        Option = option;
    }

    public string Option { get; }
}
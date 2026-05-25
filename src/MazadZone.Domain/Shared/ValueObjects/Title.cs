using MazadZone.Domain.Shared.Errors;

namespace MazadZone.Domain.Shared.ValueObjects;

// Notice we drop the inheritance. The record keyword handles value equality for us.
public sealed record Title
{
    public Title(){}
    public const int MaxLength = 100;

    public string Value { get; }

    // Private constructor enforces creation through the factory method
    private Title(string value)
    {
        Value = value;
    }

    public static Result<Title> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return TitleErrors.Empty;
        }

        if (title.Length > MaxLength)
        {
            return TitleErrors.TooLong;
        }

        return new Title(title.Trim());
    }

    public static Title FromDatabase(string value) => new Title(value ?? string.Empty);

        
}
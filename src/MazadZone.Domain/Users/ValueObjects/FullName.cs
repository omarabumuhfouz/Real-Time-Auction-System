using AuthService.Domain.Constants; 
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Domain.ValueObjects;

public record FullName
{
    private FullName() { } 

    private FullName(string first, string second, string third, string last)
    {
        FirstName = first;
        SecondName = second;
        ThirdName = third;
        LastName = last;
    }

    public string FirstName { get; init; }
    public string SecondName { get; init; } 
    public string ThirdName { get; init; }  
    public string LastName { get; init; }

    // Business Logic: Generate the display name dynamically
    public string GetDisplayName()
    {
        var parts = new[] { FirstName, SecondName, ThirdName, LastName };
        return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    public static Result<FullName> Create(string first, string second, string third, string last)
    {
        var fName = first?.Trim();
        var sName = second?.Trim();
        var tName = third?.Trim();
        var lName = last?.Trim();

        if (string.IsNullOrWhiteSpace(fName)) return FullNameErrors.FirstNameEmpty;
        if (string.IsNullOrWhiteSpace(sName)) return FullNameErrors.SecondNameEmpty;
        if (string.IsNullOrWhiteSpace(tName)) return FullNameErrors.ThirdNameEmpty;
        if (string.IsNullOrWhiteSpace(lName)) return FullNameErrors.LastNameEmpty;

        if (fName.Length > UserConstants.NameMaxLength) return FullNameErrors.FirstNameTooLong;
        if (sName.Length > UserConstants.NameMaxLength) return FullNameErrors.SecondNameTooLong;
        if (tName.Length > UserConstants.NameMaxLength) return FullNameErrors.ThirdNameTooLong;
        if (lName.Length > UserConstants.NameMaxLength) return FullNameErrors.LastNameTooLong;

        return new FullName(fName, sName, tName, lName);
    }
}
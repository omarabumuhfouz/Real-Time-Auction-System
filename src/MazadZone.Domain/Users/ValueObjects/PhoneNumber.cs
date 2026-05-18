using AuthService.Domain.Constants;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Domain.Users.ValueObjects;

public record PhoneNumber
{
    private PhoneNumber() { } 

    private PhoneNumber(string value)
        => Value = value;

    public string Value { get; init; }
    public static Result<PhoneNumber> Create(string number)
    {
        var cleanNumber = number?.Trim();

        if (string.IsNullOrWhiteSpace(cleanNumber)) return PhoneNumberErrors.Empty;

        if (cleanNumber.Length != UserConstants.PhoneNumberLength) return PhoneNumberErrors.InvalidLength;

        if (!cleanNumber.All(char.IsDigit)) return PhoneNumberErrors.InvalidFormat;

        return new PhoneNumber(cleanNumber);
    }
}
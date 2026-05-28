using AuthService.Domain.Constants;
using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.ValueObjects;
using Shouldly;

namespace Tests.Domain.Users.ValueObjects;

public class PhoneNumberTests
{
    #region 1. Empty / Whitespace Scenarios

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnEmptyError_When_PhoneNumberIsWhitespaceOrNull(string? invalidNumber)
    {
        // Act
        Result<PhoneNumber> result = PhoneNumber.Create(invalidNumber!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(PhoneNumberErrors.Empty);
    }

    #endregion

    #region 2. Length Restriction Scenarios

    [Fact]
    public void Create_Should_ReturnInvalidLengthError_When_PhoneNumberIsTooShort()
    {
        // Arrange - Dynamically build a numeric string shorter than the required length
        string tooShortNumber = new string('7', UserConstants.PhoneNumberLength - 1);

        // Act
        Result<PhoneNumber> result = PhoneNumber.Create(tooShortNumber);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(PhoneNumberErrors.InvalidLength);
    }

    // [Fact]
    // public void Create_Should_ReturnInvalidLengthError_When_PhoneNumberIsTooLong()
    // {
    //     // Arrange - Dynamically build a numeric string longer than the required length
    //     string tooLongNumber = new string('7', UserConstants.PhoneNumberLength + 1);

    //     // Act
    //     Result<PhoneNumber> result = PhoneNumber.Create(tooLongNumber);

    //     // Assert
    //     result.IsFailure.ShouldBeTrue();
    //     result.TopError.ShouldBe(PhoneNumberErrors.InvalidLength);
    // }

    #endregion

    #region 3. Format Validation Scenarios

    [Theory]
[InlineData("a")]
[InlineData("-")]
[InlineData("+")]
[InlineData(" ")] // This will no longer be stripped by Trim()!
public void Create_Should_ReturnInvalidFormatError_When_PhoneNumberContainsNonDigits(string invalidChar)
{
    // Arrange - Place the invalid character safely in the middle of the string
    int halfLength = UserConstants.PhoneNumberLength / 2;
    string leftPart = new string('7', halfLength);
    string rightPart = new string('7', UserConstants.PhoneNumberLength - halfLength - 1);
    
    string malformedNumber = leftPart + invalidChar + rightPart;

    // Act
    Result<PhoneNumber> result = PhoneNumber.Create(malformedNumber);

    // Assert
    result.IsFailure.ShouldBeTrue();
    result.TopError.ShouldBe(PhoneNumberErrors.InvalidFormat);
}

    #endregion

    #region 4. Valid Scenarios & Sanitation

    [Fact]
    public void Create_Should_ReturnSuccess_When_PhoneNumberIsValidAndNumeric()
    {
        // Arrange
        string validRawNumber = new string('7', UserConstants.PhoneNumberLength);

        // Act
        Result<PhoneNumber> result = PhoneNumber.Create(validRawNumber);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Value.ShouldBe(validRawNumber);
    }

    [Fact]
    public void Create_Should_TrimSurroundingWhitespace_Before_ValidatingLengthAndFormat()
    {
        // Arrange
        string cleanNumber = new string('7', UserConstants.PhoneNumberLength);
        string paddedNumber = $"  \t {cleanNumber}  \n ";

        // Act
        Result<PhoneNumber> result = PhoneNumber.Create(paddedNumber);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(cleanNumber);
    }

    #endregion

    #region 5. Structural Value Object Equality Scenarios

    [Fact]
    public void PhoneNumbers_Should_BeEqual_When_TheyShareTheSameUnderlyingValue()
    {
        // Arrange
        string numberString = new string('7', UserConstants.PhoneNumberLength);
        var phoneNumberOne = PhoneNumber.Create(numberString).Value;
        var phoneNumberTwo = PhoneNumber.Create($"  {numberString}  ").Value; // Padded variant will be trimmed

        // Act & Assert
        phoneNumberOne.ShouldBe(phoneNumberTwo);
        (phoneNumberOne == phoneNumberTwo).ShouldBeTrue();
    }

    #endregion
}
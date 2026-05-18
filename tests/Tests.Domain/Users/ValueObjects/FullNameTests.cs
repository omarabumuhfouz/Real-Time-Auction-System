using AuthService.Domain.Constants;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.ValueObjects;
using Shouldly;

namespace Tests.Domain.ValueObjects;

public class FullNameTests
{
    private const string ValidFirst = "Omar";
    private const string ValidSecond = "Ahmad";
    private const string ValidThird = "Ali";
    private const string ValidLast = "Al-Saeed";

    #region 1. Empty and Whitespace Validation Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnFirstNameEmpty_When_FirstNameIsInvalid(string? input)
    {
        var result = FullName.Create(input!, ValidSecond, ValidThird, ValidLast);

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(FullNameErrors.FirstNameEmpty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnSecondNameEmpty_When_SecondNameIsInvalid(string? input)
    {
        var result = FullName.Create(ValidFirst, input!, ValidThird, ValidLast);

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(FullNameErrors.SecondNameEmpty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnThirdNameEmpty_When_ThirdNameIsInvalid(string? input)
    {
        var result = FullName.Create(ValidFirst, ValidSecond, input!, ValidLast);

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(FullNameErrors.ThirdNameEmpty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnLastNameEmpty_When_LastNameIsInvalid(string? input)
    {
        var result = FullName.Create(ValidFirst, ValidSecond, ValidThird, input!);

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(FullNameErrors.LastNameEmpty);
    }

    #endregion

    #region 2. Max Length Validation Tests

    [Fact]
    public void Create_Should_ReturnFirstNameTooLong_When_FirstNameExceedsLimit()
    {
        var longName = new string('a', UserConstants.NameMaxLength + 1);
        var result = FullName.Create(longName, ValidSecond, ValidThird, ValidLast);

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(FullNameErrors.FirstNameTooLong);
    }

    [Fact]
    public void Create_Should_ReturnSecondNameTooLong_When_SecondNameExceedsLimit()
    {
        var longName = new string('a', UserConstants.NameMaxLength + 1);
        var result = FullName.Create(ValidFirst, longName, ValidThird, ValidLast);

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(FullNameErrors.SecondNameTooLong);
    }

    [Fact]
    public void Create_Should_ReturnThirdNameTooLong_When_ThirdNameExceedsLimit()
    {
        var longName = new string('a', UserConstants.NameMaxLength + 1);
        var result = FullName.Create(ValidFirst, ValidSecond, longName, ValidLast);

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(FullNameErrors.ThirdNameTooLong);
    }

    [Fact]
    public void Create_Should_ReturnLastNameTooLong_When_LastNameExceedsLimit()
    {
        var longName = new string('a', UserConstants.NameMaxLength + 1);
        var result = FullName.Create(ValidFirst, ValidSecond, ValidThird, longName);

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(FullNameErrors.LastNameTooLong);
    }

    #endregion

    #region 3. Trimming and Assignment Properties Tests

    [Fact]
    public void Create_Should_TrimInputs_When_ValuesHaveSurroundingSpaces()
    {
        // Arrange
        var spacesFirst = "  Omar  ";
        var spacesSecond = " Ahmad ";
        var spacesThird = "\tAli\n";
        var spacesLast = "Al-Saeed  ";

        // Act
        var result = FullName.Create(spacesFirst, spacesSecond, spacesThird, spacesLast);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.FirstName.ShouldBe(ValidFirst);
        result.Value.SecondName.ShouldBe(ValidSecond);
        result.Value.ThirdName.ShouldBe(ValidThird);
        result.Value.LastName.ShouldBe(ValidLast);
    }

    #endregion

    #region 4. Display Name Business Logic Tests

    [Fact]
    public void GetDisplayName_Should_ReturnCleanFormattedFourPartFullName()
    {
        // Arrange
        var fullName = FullName.Create(ValidFirst, ValidSecond, ValidThird, ValidLast).Value;

        // Act
        var displayName = fullName.GetDisplayName();

        // Assert
        displayName.ShouldBe("Omar Ahmad Ali Al-Saeed");
    }
    
    #endregion
}
using AuthService.Domain.Constants;
using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Users.ValueObjects;
using Shouldly;

namespace Tests.Domain.Users.ValueObjects;

public class EmailTests
{
    #region 1. Empty / Whitespace Scenarios

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnEmptyError_When_EmailIsWhitespaceOrNull(string? invalidEmail)
    {
        // Act
        Result<Email> result = Email.Create(invalidEmail!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(EmailErrors.Empty);
    }

    #endregion

    #region 2. Length Restriction Scenarios

    [Fact]
    public void Create_Should_ReturnTooLongError_When_EmailExceedsMaxLength()
    {
        // Arrange
        // Create a string that dynamically breaches your configured UserConstants limit
        string domain = "@example.com";
        int repeatCount = UserConstants.EmailMaxLength - domain.Length + 1;
        string excessiveEmail = new string('a', repeatCount) + domain;

        // Act
        Result<Email> result = Email.Create(excessiveEmail);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(EmailErrors.TooLong);
    }

    #endregion

    #region 3. Invalid Format Scenarios (Regex Check)

    [Theory]
    [InlineData("plainaddress")]               // No @ symbol
    [InlineData("@missingusername.com")]        // No local part
    [InlineData("username@")]                  // No domain part
    [InlineData("username@domain..com")]       // Double dots in domain
    [InlineData("username@domain,com")]        // Comma instead of dot
    [InlineData("username @domain.com")]       // Spaces inside address
    [InlineData("username@domain .com")]       // Spaces inside domain
    public void Create_Should_ReturnInvalidFormatError_When_RegexDoesNotMatch(string malformedEmail)
    {
        // Act
        Result<Email> result = Email.Create(malformedEmail);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(EmailErrors.InvalidFormat);
    }

    #endregion

    #region 4. Valid Scenarios & Normalization

    [Theory]
    [InlineData("omar@au.edu.jo")]
    [InlineData("developer.test@gmail.com")]
    [InlineData("alex+filter@subdomain.domain.org")]
    [InlineData("123456@numericlocal.net")]
    public void Create_Should_ReturnSuccess_When_EmailFormatIsValid(string validEmail)
    {
        // Act
        Result<Email> result = Email.Create(validEmail);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Value.ShouldBe(validEmail);
    }

    [Fact]
    public void Create_Should_NormalizeEmailToLowerCase_When_InputHasUpperCases()
    {
        // Arrange
        string mixedCaseEmail = "OmAr.DeVeLoPeR@GmaIL.CoM";
        string expectedLower = "omar.developer@gmail.com";

        // Act
        Result<Email> result = Email.Create(mixedCaseEmail);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(expectedLower);
    }

    #endregion

    #region 5. Implicit Operators & String Conversion

    [Fact]
    public void ImplicitOperator_And_ToString_Should_ReturnTheRawUnderlyingValue()
    {
        // Arrange
        string rawInput = "test@domain.com";
        Email email = Email.Create(rawInput).Value;

        // Act
        string implicitString = email; // Testing implicit operator assignment
        string toStringResult = email.ToString(); // Testing ToString override

        // Assert
        implicitString.ShouldBe(rawInput);
        toStringResult.ShouldBe(rawInput);
    }

    #endregion
}
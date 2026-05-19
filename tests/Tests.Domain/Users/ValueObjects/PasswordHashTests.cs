using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Users.ValueObjects;
using Shouldly;

namespace Tests.Domain.Users.ValueObjects;

public class PasswordHashTests
{
    private const string DummyHash = "$2a$12$R9h/lSBaCR9xlBq6Z.a6COEa2vJw6.E8F/.C3PZpH7tH7D6bZc3Ky";

    #region 1. Validation Constraints

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnValidationError_When_HashIsEmptyOrWhitespace(string? invalidHash)
    {
        // Act
        var result = PasswordHash.Create(invalidHash!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.Code.ShouldBe("PasswordHash.Empty");
        result.TopError.Message.ShouldBe("The password hash cannot be empty.");
        result.TopError.Type.ShouldBe(ErrorType.Validation);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_HashIsProvided()
    {
        // Act
        var result = PasswordHash.Create(DummyHash);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Value.ShouldBe(DummyHash);
    }

    #endregion

    #region 2. Security Controls

    [Fact]
    public void ToString_Should_ReturnMaskedString_ToPreventAccidentalLogging()
    {
        // Arrange
        var passwordHash = PasswordHash.Create(DummyHash).Value;

        // Act
        var outputString = passwordHash.ToString();

        // Assert
        outputString.ShouldBe("***");
        outputString.ShouldNotContain(DummyHash);
    }

    #endregion

    #region 3. Value Object Structural Properties

    [Fact]
    public void Records_Should_BeEqual_When_TheyContainTheSameHashValue()
    {
        // Arrange
        var hashOne = PasswordHash.Create(DummyHash).Value;
        var hashTwo = PasswordHash.Create(DummyHash).Value;

        // Act & Assert
        hashOne.ShouldBe(hashTwo); // Tests structural record equality
        (hashOne == hashTwo).ShouldBeTrue();
    }

    #endregion
}
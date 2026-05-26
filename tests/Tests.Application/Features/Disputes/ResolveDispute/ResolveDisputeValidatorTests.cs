using FluentValidation.TestHelper;
using MazadZone.Application.Features.Orders.Commands.ResolveDispute;
using MazadZone.Application.Orders.ResolveDispute;
using MazadZone.Domain.Disputes;
using MazadZone.Domain.Orders; // Adjust if DisputeId is in a different namespace

namespace Tests.Application.Features.Orders.Commands.ResolveDispute;

public class ResolveDisputeValidatorTests
{
    private readonly ResolveDisputeValidator _validator;

    public ResolveDisputeValidatorTests()
    {
        _validator = new ResolveDisputeValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = OrderHelper.CreateResolveDisputeCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDisputeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        // Note: Corrected from OrderId to DisputeId to match the validator
        var command = OrderHelper.CreateResolveDisputeCommand() with { DisputeId = DisputeId.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisputeId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenResolutionIsNullOrWhitespace_ShouldHaveValidationError(string? invalidResolution)
    {
        // Arrange
        var command = OrderHelper.CreateResolveDisputeCommand() with { Resolution = invalidResolution };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Resolution);
    }

    [Fact]
    public void Validate_WhenResolutionIsTooShort_ShouldHaveValidationError()
    {
        // Arrange
        // This assumes MustBeValidResolution requires a specific minimum length
        var command = OrderHelper.CreateResolveDisputeCommand() with { Resolution = "Too short" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Resolution);
    }
}
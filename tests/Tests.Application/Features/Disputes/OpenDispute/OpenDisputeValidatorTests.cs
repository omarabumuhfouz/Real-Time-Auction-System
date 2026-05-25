using MazadZone.Application.Features.Disputes.Commands.OpenDispute;
using MazadZone.Domain.Orders;
using Tests.Application.Features.Orders;

namespace Tests.Application.Orders.OpenDispute;

public class OpenDisputeValidatorTests
{
    private readonly OpenDisputeValidator _validator;

    public OpenDisputeValidatorTests()
    {
        _validator = new OpenDisputeValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = OrderHelper.CreateOpenDisputeCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OrderIdIsEmpty_FailsValidation()
    {
        // Arrange
        var command = OrderHelper.CreateOpenDisputeCommand() with { OrderId = OrderId.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public void Validate_DisputeTypeIdIsEmpty_FailsValidation()
    {
        // Arrange
        var command = OrderHelper.CreateOpenDisputeCommand() with { DisputeTypeId = DisputeTypeId.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisputeTypeId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_TitleIsNullOrWhitespace_FailsValidation(string? invalidTitle)
    {
        // Arrange
        var command = OrderHelper.CreateOpenDisputeCommand() with { Title = invalidTitle! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_DescriptionIsNullOrWhitespace_FailsValidation(string? invalidDescription)
    {
        // Arrange
        var command = OrderHelper.CreateOpenDisputeCommand() with { Description = invalidDescription! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
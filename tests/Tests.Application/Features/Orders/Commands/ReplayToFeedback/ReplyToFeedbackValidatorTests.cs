using MazadZone.Application.Features.Orders.Commands.ReplyToFeedback;
using MazadZone.Domain.Orders;

namespace Tests.Application.Features.Orders.Commands.ReplyToFeedback;

public class ReplyToFeedbackValidatorTests
{
    private readonly ReplyToFeedbackValidator _validator;

    public ReplyToFeedbackValidatorTests()
    {
        _validator = new ReplyToFeedbackValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new ReplyToFeedbackCommand(OrderId.New(), "Thank you for the positive feedback!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OrderIdIsEmpty_FailsValidation()
    {
        // Arrange
        var command = new ReplyToFeedbackCommand(OrderId.Empty, "Valid reply text");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
        
        // Ensure the valid reply text didn't falsely trigger an error
        result.ShouldNotHaveValidationErrorFor(x => x.ReplyText);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReplyTextIsNullOrWhitespace_FailsValidation(string? invalidReply)
    {
        // Arrange
        var command = new ReplyToFeedbackCommand(OrderId.New(), invalidReply!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();

        // Verify the exact custom message you defined
        result.ShouldHaveValidationErrorFor(x => x.ReplyText);
    }

    [Fact]
    public void Validate_ReplyTextExceedsMaxLength_FailsValidation()
    {
        // Arrange
        // Dynamically create a string that is exactly 1 character over your domain limit
        var tooLongReply = new string('A', OrderConstants.MaxCommentLength + 1);
        var command = new ReplyToFeedbackCommand(OrderId.New(), tooLongReply);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();

        // Verify the dynamically injected message
        result.ShouldHaveValidationErrorFor(x => x.ReplyText);
    }
}
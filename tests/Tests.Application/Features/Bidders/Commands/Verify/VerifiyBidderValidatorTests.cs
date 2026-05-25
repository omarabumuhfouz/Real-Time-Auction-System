using MazadZone.Application.Features.Bidders.Commands.Verify;
using MazadZone.Domain.Bidders;

namespace Tests.Application.Features.Bidders.Commands.Verify;

public class VerifyBidderCommandValidatorTests
{
    private readonly VerifyBidderCommandValidator _validator;

    public VerifyBidderCommandValidatorTests()
    {
        _validator = new VerifyBidderCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = BidderHelper.CreateVerifyBidderCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_BidderIdIsEmpty_FailsValidation()
    {
        // Arrange
        var command = BidderHelper.CreateVerifyBidderCommand() with { BidderId = UserId.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        
        // Verifies that your MustBeValidBidderId() extension caught the bad input
        result.ShouldHaveValidationErrorFor(x => x.BidderId);
    }
}
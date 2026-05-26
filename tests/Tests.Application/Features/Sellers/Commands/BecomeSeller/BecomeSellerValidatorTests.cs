using MazadZone.Application.Features.Sellers.Commands.BecomeSeller;

namespace Tests.Application.Features.Sellers.Commands.BecomeSeller;

public class BecomeSellerValidatorTests
{
    private readonly BecomeSellerValidator _validator;

    public BecomeSellerValidatorTests()
    {
        _validator = new BecomeSellerValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = SellerHelper.CreateBecomeSellerCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_UserIdIsEmpty_FailsValidation()
    {
        // Arrange
        var command = SellerHelper.CreateBecomeSellerCommand() with { UserId = UserId.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

   
}
using MazadZone.Application.Features.Users.Commands.Ban;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.ValueObjects;
using MediatR;

namespace Tests.Application.Features.Users.Commands.Ban;

public class BanUserCommandHandlerTests : UserBaseTest<BanUserCommandHandler>
{
    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = new BanUserCommand(UserId.New(), "Violation of terms.");

        // Safe setup using the primitive .Value 
        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);

        // Verify that database operations were completely bypassed
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReasonIsInvalid_ReturnsValidationError()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        
        // Pass an empty string to force Reason.Create() to fail its domain validation
        var command = new BanUserCommand(user.Id, string.Empty);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // It shouldn't be a user error, it should be whatever error your Reason VO returns
        result.TopError.ShouldNotBe(UserErrors.NotFound); 

        // Verify the user state wasn't mutated and DB wasn't hit
        user.Status.ShouldBe(UserStatus.Active);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_BansUserAndSavesChanges()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        var banReason = "Severe policy violation: Fraudulent bids.";
        var command = new BanUserCommand(user.Id, banReason);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);

        // Verify aggregate state was mutated successfully
        user.Status.ShouldBe(UserStatus.Banned);
        
        // Verify we passed the exact reason string down to the domain aggregate
        user.EnforcementReason.ShouldNotBeNull();
        user.EnforcementReason.Text.ShouldBe(banReason);

        // Verify the transaction was committed
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
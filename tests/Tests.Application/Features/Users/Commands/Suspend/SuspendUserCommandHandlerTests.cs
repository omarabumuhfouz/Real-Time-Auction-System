using MazadZone.Application.Features.Users.Commands.Suspend;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.ValueObjects;
using MediatR;

namespace Tests.Application.Features.Users.Commands.Suspend;

public class SuspendUserCommandHandlerTests : UserBaseTest<SuspendUserCommandHandler>
{
    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = CreateCommand();

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);

        // Verify database operations were completely bypassed
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReasonIsInvalid_ReturnsValidationError()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        
        // Pass an empty string to force Reason.Create() to fail its domain validation
        var command = CreateCommand(user.Id, string.Empty);

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldNotBe(UserErrors.NotFound); // Will be your specific Reason VO error

        // Verify user state wasn't mutated
        user.Status.ShouldBe(UserStatus.Active);
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserIsBanned_ReturnsDomainError()
    {
        // Arrange - Force a state invariant failure by trying to suspend an already Banned user
        var user = UserHelper.CreateBannedUser();
        var command = CreateCommand(user.Id);

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Ensure the error returned matches your domain's specific invariant error
        // (e.g., UserErrors.CannotSuspendBannedUser)
        result.TopError.Code.ShouldNotBe(UserErrors.NotFound.Code);

        // Verify database writes were skipped because the domain logic rejected the action
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_SuspendsUserAndSavesChanges()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();

        var command = CreateCommand(user.Id);

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);

        // Verify aggregate state was mutated successfully
        user.Status.ShouldBe(UserStatus.Suspended);
        user.SuspensionUntil.ShouldBe(command.Until);
        user.EnforcementReason.ShouldNotBeNull();
        user.EnforcementReason.Text.ShouldBe(command.Reason);

        // Verify repository interaction and unit of work transaction commits
        _userRepository.Received(1).Update(Arg.Is<User>(u => u.Id.Value == user.Id.Value));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private SuspendUserCommand CreateCommand(UserId? userId = null, string? reason = null, DateTime? suspendUntil = null)
    {
        return new SuspendUserCommand(
            userId ?? UserId.New(),
            reason ?? "Violation of terms of service.",
            suspendUntil ?? DateTime.UtcNow.AddDays(7));
    }
}
using MazadZone.Application.Features.Users.Commands.Activate;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.ValueObjects;
using MediatR;

namespace Tests.Application.Features.Users.Commands.Activate;

public class ActivateUserCommandHandlerTests : UserBaseTest<ActivateUserCommandHandler>
{
    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = new ActivateUserCommand(UserId.New());

        // Setup repository mock using the primitive .Value to guarantee a perfect match
        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);

        // Verify that persistence operations were completely blocked
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserIsBanned_ReturnsCannotActivateBannedUserError()
    {
        var user = UserHelper.CreateBannedUser();
        var command = new ActivateUserCommand(user.Id);

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.CannotActivateBannedUser);

        // Verify database writes were skipped because the domain logic failed
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserIsSuspended_ActivatesUserAndSavesChanges()
    {
        // Arrange - Set up an eligible suspended user
        var user = UserHelper.CreateSuspendedUser();
        var command = new ActivateUserCommand(user.Id);

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);

        // Verify aggregate tracking states changed properly
        user.Status.ShouldBe(UserStatus.Active);
        user.SuspensionUntil.ShouldBeNull();

        // Verify repository interaction and unit of work transaction commits
        _userRepository.Received(1).Update(Arg.Is<User>(u => u.Id.Value == user.Id.Value));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
using AuthService.Application.Features.Users.Commands.ChangePassword;
using MazadZone.Application.Features.Users.Commands.ChangePassword;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.ValueObjects;
using MediatR;

namespace Tests.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandlerTests : UserBaseTest<ChangePasswordCommandHandler>
{
    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = new ChangePasswordCommand(UserId.New(), "oldPass123!", "newPass123!", "newPass123!");

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);

        // Ensure we aborted early
        _passwordService.DidNotReceiveWithAnyArgs().ValidatePassword(default!, default!);
    }

    [Fact]
    public async Task Handle_CurrentPasswordIsIncorrect_ReturnsInvalidCredentialsError()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        var command = new ChangePasswordCommand(user.Id, "wrongOldPassword", "newPass123!", "newPass123!");

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns(user);

        // Setup the password service to reject the current password
        _passwordService.ValidatePassword(command.CurrentPassword, user.PasswordHash.Value)
            .Returns(false);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.InvalidCredentials);

        // Ensure we didn't attempt to hash the new password
        _passwordService.DidNotReceiveWithAnyArgs().HashPassword(default!);
    }

    [Fact]
    public async Task Handle_PasswordHashCreationFails_ReturnsInvalidCredentialsError()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        var command = new ChangePasswordCommand(user.Id, "correctOldPass", "newPass123!", "newPass123!");

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordService.ValidatePassword(command.CurrentPassword, user.PasswordHash.Value)
            .Returns(true);

        // Simulate a failure in the hashing service returning an empty string, 
        // which trips the PasswordHash.Create validation guard.
        _passwordService.HashPassword(command.NewPassword)
            .Returns(string.Empty);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Your handler explicitly maps domain hashing failures to InvalidCredentials
        result.TopError.ShouldBe(UserErrors.InvalidCredentials); 

        // Ensure state wasn't updated
        _userRepository.DidNotReceiveWithAnyArgs().Update(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_ChangesPasswordAndSavesChanges()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        var command = new ChangePasswordCommand(user.Id, "correctOldPass", "newPass123!", "newPass123!");
        var expectedNewHash = "$2a$12$NewValidHashStringGoesHere123456789";

        _userRepository.GetByIdAsync(command.UserId.Value, Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordService.ValidatePassword(command.CurrentPassword, user.PasswordHash.Value)
            .Returns(true);

        _passwordService.HashPassword(command.NewPassword)
            .Returns(expectedNewHash);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);

        // Verify the user aggregate state was modified
        user.PasswordHash.Value.ShouldBe(expectedNewHash);

        // Verify persistence calls were made
        _userRepository.Received(1).Update(Arg.Is<User>(u => u.Id.Value == user.Id.Value));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
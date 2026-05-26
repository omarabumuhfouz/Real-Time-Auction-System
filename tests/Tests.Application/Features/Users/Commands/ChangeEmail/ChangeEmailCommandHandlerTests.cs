using MazadZone.Application.Features.Users.Commands.ChangeEmail;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.ValueObjects;
using MediatR;

namespace Tests.Application.Features.Users.Commands.ChangeEmail;

public class ChangeEmailCommandHandlerTests : UserBaseTest<ChangeEmailCommandHandler>
{
    [Fact]
    public async Task Handle_EmailFormatIsInvalid_ReturnsValidationError()
    {
        // Arrange
        // Passing a malformed email string to trigger the Value Object validation failure
        var command = new ChangeEmailCommand(UserId.New(), "invalid-email-format");

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Ensure it failed before hitting the database
        await _userRepository.DidNotReceiveWithAnyArgs().IsEmailInUseAsync(default!, default);
        await _userRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(command.UserId, default);
    }

    [Fact]
    public async Task Handle_EmailIsAlreadyInUse_ReturnsAlreadyInUseError()
    {
        // Arrange
        var targetEmail = "taken@mazadzone.com";
        var command = new ChangeEmailCommand(UserId.New(), targetEmail);

        // Setup the repository to simulate that this email is already claimed
        _userRepository.IsEmailInUseAsync(
                Arg.Is<Email>(e => e.Value == targetEmail), 
                Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(EmailErrors.AlreadyInUse);

        // Ensure we aborted before trying to load the user or save changes
        await _userRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(command.UserId, default);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = new ChangeEmailCommand(UserId.New(), "available@mazadzone.com");

        // Setup: Email is free, but the User ID is not found
        _userRepository.IsEmailInUseAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);

        // Ensure no state mutations were persisted
        _userRepository.DidNotReceiveWithAnyArgs().Update(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesEmailAndSavesChanges()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        var newEmailString = "updated@mazadzone.com";
        var command = new ChangeEmailCommand(user.Id, newEmailString);

        // Setup: Email is unique, and user is found
        _userRepository.IsEmailInUseAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);

        // Verify the aggregate state was properly mutated
        user.Email.Value.ShouldBe(newEmailString);

        // Verify database persistence steps were executed
        _userRepository.Received(1).Update(Arg.Is<User>(u => u.Id.Value == user.Id.Value));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
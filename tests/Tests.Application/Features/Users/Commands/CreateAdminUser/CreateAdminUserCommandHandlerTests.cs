namespace Tests.Application.Users.Commands.CreateAdminUser;

using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Users.Commands.CreateAdminUser;
using MazadZone.Domain.Users;
using Tests.Application.Features.Users;

public class CreateAdminUserCommandHandlerTests  : UserBaseTest<CreateAdminUserCommandHandler>
{
    [Fact]
    public async Task Handle_InvalidEmailFormat_ReturnsDomainError()
    {
        // Arrange
        var command = new CreateAdminUserCommand(
            "invalid-email", "Password123!", "1234567890", "First", "Second", "Third", "Last");

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Verify we aborted early
        await _userRepository.DidNotReceiveWithAnyArgs().IsEmailInUseAsync(default!, default);
        _userRepository.DidNotReceiveWithAnyArgs().Add(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUse_ReturnsEmailAlreadyInUseError()
    {
        // Arrange
        var command = new CreateAdminUserCommand(
            "existing@example.com", "Password123!", "1234567890", "First", "Second", "Third", "Last");

        _userRepository.IsEmailInUseAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(EmailErrors.AlreadyInUse); // Assumes EmailErrors.AlreadyInUse exists

        // Verify we aborted before hashing or saving
        _passwordService.DidNotReceiveWithAnyArgs().HashPassword(default!);
        _userRepository.DidNotReceiveWithAnyArgs().Add(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_DomainValidationFails_ReturnsDomainError()
    {
        // Arrange
        // Passing an empty string for the phone number to force User.Create() to fail
        var command = new CreateAdminUserCommand(
            "new@example.com", "Password123!", string.Empty, "First", "Second", "Third", "Last");

        _userRepository.IsEmailInUseAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _passwordService.HashPassword(command.Password).Returns("hashed_password");

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();

        // Verify we aborted before saving
        _userRepository.DidNotReceiveWithAnyArgs().Add(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAdminUserAndSavesChanges()
    {
        // Arrange
        var command = new CreateAdminUserCommand(
            "admin@example.com", "Password123!", "1234567890", "Admin", "Super", "User", "Account");

        _userRepository.IsEmailInUseAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _passwordService.HashPassword(command.Password).Returns("hashed_password");

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty); // Verify it returns a valid Guid ID

        // Verify the password service was called
        _passwordService.Received(1).HashPassword(command.Password);

        // Verify the repository Add was called with a user that has the Admin role
        _userRepository.Received(1).Add(Arg.Is<User>(u => u.IsAdmin));

        // Verify the infrastructure commit was called
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
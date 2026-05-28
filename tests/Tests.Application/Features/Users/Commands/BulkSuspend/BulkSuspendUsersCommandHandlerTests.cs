namespace Tests.Application.Features.Users.Commands.BulkSuspend;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Features.Users.Commands.BulkSuspend;
using MazadZone.Domain.Users.Entities;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users;

// using MazadZone.Domain.Users.ValueObjects; // Where UserId and Reason live

public class BulkSuspendUsersCommandHandlerTests : UserBaseTest<BulkSuspendUsersCommandHandler>
{
    [Fact]
    public async Task Handle_InvalidReason_ReturnsDomainError()
    {
        // Arrange
        var command = new BulkSuspendUsersCommand(
            new List<UserId> { UserId.New() }, 
            string.Empty, // Invalid reason
            DateTime.UtcNow.AddDays(7));

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Verify we aborted before hitting the database or saving
        await _userRepository.DidNotReceiveWithAnyArgs().GetByIdsAsync(default!, default);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_NoUsersFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new BulkSuspendUsersCommand(
            new List<UserId> { UserId.New(), UserId.New() }, 
            "Suspicious activity", 
            DateTime.UtcNow.AddDays(7));

        // Mock repo to return an empty list
        _userRepository.GetByIdsAsync(command.UserIds, Arg.Any<CancellationToken>())
            .Returns(new List<User>());

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);

        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_DomainValidationFailsForAnyUser_AbortsAndReturnsDomainError()
    {
        // Arrange
        var command = new BulkSuspendUsersCommand(
            new List<UserId> { UserId.New(), UserId.New() }, 
            "Suspicious activity", 
            DateTime.UtcNow.AddDays(7));

        var validUser = UserHelper.CreateActiveUser();

        // Create an invalid user (e.g., already banned, so Suspend() will fail based on your domain rules)
        var invalidUser = UserHelper.CreateBannedUser();
        
        var usersList = new List<User> { validUser, invalidUser };

        _userRepository.GetByIdsAsync(command.UserIds, Arg.Any<CancellationToken>())
            .Returns(usersList);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Verify the All-or-Nothing transaction worked: it aborted before calling SaveChanges
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_SuspendsAllUsersAndSavesChanges()
    {
        // Arrange
        var command = new BulkSuspendUsersCommand(
            new List<UserId> { UserId.New(), UserId.New() }, 
            "Suspicious bidding patterns", 
            DateTime.UtcNow.AddDays(7));

        var user1 = UserHelper.CreateActiveUser();
        var user2 = UserHelper.CreateActiveUser();
        
        var usersList = new List<User> { user1, user2 };

        _userRepository.GetByIdsAsync(command.UserIds, Arg.Any<CancellationToken>())
            .Returns(usersList);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // 💡 Verify the domain state actually mutated for both users
        user1.Status.ShouldBe(UserStatus.Suspended); 
        user2.Status.ShouldBe(UserStatus.Suspended);
        
        user1.EnforcementReason.Text.ShouldBe(command.Reason);
        user1.SuspensionUntil.ShouldBe(command.Until);

        // Verify the infrastructure commit was called exactly once to save the bulk changes
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
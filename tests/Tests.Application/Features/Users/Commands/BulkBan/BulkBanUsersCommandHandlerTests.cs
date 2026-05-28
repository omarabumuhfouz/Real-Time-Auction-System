namespace Tests.Application.Features.Users.Commands.BulkBan;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Features.Users.Commands.BulkBan;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users;


public class BulkBanUsersCommandHandlerTests : UserBaseTest<BulkBanUsersCommandHandler>
{
    [Fact]
    public async Task Handle_InvalidReason_ReturnsDomainError()
    {
        // Arrange
        // Passing an empty string to force Reason.Create() to fail
        var command = new BulkBanUsersCommand(new List<UserId> { UserId.New() }, string.Empty);

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
        var command = new BulkBanUsersCommand(new List<UserId> { UserId.New(), UserId.New() }, "Violated TOS");

        // Mock repo to return an empty list
        _userRepository.GetByIdsAsync(command.UserIds, Arg.Any<CancellationToken>())
            .Returns(new List<User>());

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);

        // Verify we aborted before saving
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_BansAllUsersAndSavesChanges()
    {
        // Arrange
        var command = new BulkBanUsersCommand(
            new List<UserId> { UserId.New(), UserId.New() }, 
            "Fraudulent activity");

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
        // Because your User.Ban() is idempotent and handles its own state, we check that it applied.
        user1.Status.ShouldBe(UserStatus.Banned); 
        user2.Status.ShouldBe(UserStatus.Banned);
        
        // Verify the enforcement reason was properly set from the value object
        user1.EnforcementReason.Text.ShouldBe(command.Reason);

        // Verify the infrastructure commit was called exactly once to save the bulk changes
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
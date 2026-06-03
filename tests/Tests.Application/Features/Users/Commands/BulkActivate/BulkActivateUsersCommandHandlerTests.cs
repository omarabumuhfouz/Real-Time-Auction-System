namespace Tests.Application.Features.Users.Commands.BulkActivate;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Features.Users.Commands.BulkActivate;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users;


public class BulkActivateUsersCommandHandlerTests : UserBaseTest<BulkActivateUsersCommandHandler>
{
    [Fact]
    public async Task Handle_NoUsersFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new BulkActivateUsersCommand(new List<UserId> { UserId.New(), UserId.New() });

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
    public async Task Handle_DomainValidationFailsForAnyUser_AbortsAndReturnsDomainError()
    {
        // Arrange
        var command = new BulkActivateUsersCommand(new List<UserId> { UserId.New(), UserId.New() });

        var validUser = UserHelper.CreateActiveUser();

        var bannedUser = UserHelper.CreateBannedUser();

        var usersList = new List<User> { validUser, bannedUser };

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
    public async Task Handle_ValidCommand_ActivatesAllUsersAndSavesChanges()
    {
        // Arrange
        var command = new BulkActivateUsersCommand(new List<UserId> { UserId.New(), UserId.New() });

        var user1 = UserHelper.CreateSuspendedUser();
        var user2 = UserHelper.CreateSuspendedUser();
        
        // Ensure they start in a state that requires activation (if necessary for your domain)

        var usersList = new List<User> { user1, user2 };

        _userRepository.GetByIdsAsync(command.UserIds, Arg.Any<CancellationToken>())
            .Returns(usersList);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify the domain state actually mutated for both users
        user1.Status.ShouldBe(UserStatus.Active); 
        user2.Status.ShouldBe(UserStatus.Active);

        // Verify the infrastructure commit was called exactly once to save the bulk changes
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
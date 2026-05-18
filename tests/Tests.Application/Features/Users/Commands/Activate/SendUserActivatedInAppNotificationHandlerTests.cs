using MazadZone.Application.Features.Users.Commands.Activate;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.Activate;

public class SendUserActivatedInAppNotificationHandlerTests : UserBaseTest<SendUserActivatedInAppNotificationHandler>
{
    [Fact]
    public async Task Handle_UserActivated_SendsInAppNotification()
    {
        // Arrange
        var userId = UserId.New();
        var email = "omar@mazadzone.com";
        var domainEvent = new UserActivatedDomainEvent(userId, email);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        var expectedTitle = "Welcome Back! Account Activated";
        var expectedMessage = "Your account suspension has been lifted. You can now participate in auctions again.";

        await _notificationRepository.Received(1).NotifyUserAsync(
            userId,
            expectedTitle,
            expectedMessage,
            Arg.Any<CancellationToken>());
    }
}
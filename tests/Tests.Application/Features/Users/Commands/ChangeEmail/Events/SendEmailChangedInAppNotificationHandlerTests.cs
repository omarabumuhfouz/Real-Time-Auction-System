using MazadZone.Application.Features.Users.Commands.ChangeEmail.Events;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.ChangeEmail.Events;

public class SendEmailChangedInAppNotificationHandlerTests : UserBaseTest<SendEmailChangedInAppNotificationHandler>
{
    [Fact]
    public async Task Handle_EmailChanged_SendsSecurityAlertNotification()
    {
        // Arrange
        var userId = UserId.New();
        var oldEmail = Email.Create("old.email@mazadzone.com").Value;
        var newEmail = Email.Create("new.email@mazadzone.com").Value;
        
        var domainEvent = new UserEmailChangedDomainEvent(userId, oldEmail, newEmail);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        const string expectedTitle = "Security Alert: Email Updated";

        // ✅ Verify the infrastructure mock received the exact parameters and safe value object mapping
        await _notificationRepository.Received(1).NotifyUserAsync(
            userId,
            Arg.Is<string>(title => title == expectedTitle),
            Arg.Is<string>(message => 
                message.Contains(oldEmail.Value) && 
                message.Contains(newEmail.Value) && 
                message.StartsWith("The email associated with your account was changed")),
            Arg.Any<CancellationToken>());
    }
}
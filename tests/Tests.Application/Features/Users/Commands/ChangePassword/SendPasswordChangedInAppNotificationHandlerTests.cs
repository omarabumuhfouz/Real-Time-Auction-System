using MazadZone.Application.Features.Users.Commands.ChangePassword;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;

// Namespace aligned with the typical testing structure
namespace Tests.Application.Features.Users.Commands.ChangePassword.Events;

public class SendPasswordChangedInAppNotificationHandlerTests : UserBaseTest<SendPasswordChangedInAppNotificationHandler>
{
    [Fact]
    public async Task Handle_PasswordChanged_SendsInAppNotification()
    {
        // Arrange
        var userId = UserId.New();
        var email = Email.Create("user@mazadzone.com").Value; // Required by your domain event constructor
        
        var domainEvent = new UserPasswordChangedDomainEvent(userId, email);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        const string expectedTitle = "Security Update: Password Changed";
        const string expectedMessage = "Your account password was successfully updated.";

        await _notificationRepository.Received(1).NotifyUserAsync(
            userId,
            Arg.Is<string>(t => t == expectedTitle),
            Arg.Is<string>(m => m == expectedMessage),
            Arg.Any<CancellationToken>());
    }
}
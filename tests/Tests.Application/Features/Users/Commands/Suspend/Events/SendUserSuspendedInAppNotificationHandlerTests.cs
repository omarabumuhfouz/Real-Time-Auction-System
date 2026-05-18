using MazadZone.Application.Features.Users.Commands.Suspend.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.Suspend.Events;

public class SendUserSuspendedInAppNotificationHandlerTests : UserBaseTest<SendUserSuspendedInAppNotificationHandler>
{
    [Fact]
    public async Task Handle_ReinstatementDateProvided_SendsNotificationWithDate()
    {
        // Arrange
        var userId = UserId.New();
        var reinstatementDate = DateTime.UtcNow.AddDays(5);

        var domainEvent = UserHelper.CreateSuspensionEvent(userId, reinstatementDate);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert

        await _notificationRepository.Received(1).NotifyUserAsync(
            userId,
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReinstatementDateIsNull_SendsNotificationWithIndefiniteStatus()
    {
        // Arrange
        var userId = UserId.New();
        
        // Pass null for the date to trigger the fallback logic
        var domainEvent = UserHelper.CreateSuspensionEvent(userId, null);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        await _notificationRepository.Received(1).NotifyUserAsync(
            userId,
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}
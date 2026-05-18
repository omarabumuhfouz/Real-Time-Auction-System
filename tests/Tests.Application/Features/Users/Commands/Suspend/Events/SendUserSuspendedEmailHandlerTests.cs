using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Users.Commands.Suspend.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.Suspend.Events;

public class SendUserSuspendedEmailHandlerTests : UserBaseTest<SendUserSuspendedEmailHandler>
{
    [Fact]
    public async Task Handle_ReinstatementDateProvided_SendsEmailWithDate()
    {
        // Arrange
        var userId = UserId.New();
        var reinstatementDate = DateTime.UtcNow.AddDays(7);

        var domainEvent = UserHelper.CreateSuspensionEvent(userId, reinstatementDate);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        var expectedDateString = reinstatementDate.ToShortDateString();

        // ✅ Verify the email was routed correctly and the date formatting was applied
        await _emailService.Received(1).SendEmailAsync(
            Arg.Is<EmailRequest>(req => 
                req.To == domainEvent.Email && 
                req.Subject == "Account Suspended" &&
                req.Body.Contains($"Estimated return: {expectedDateString}") &&
                req.IsHtml == false),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReinstatementDateIsNull_SendsEmailWithIndefiniteStatus()
    {
        // Arrange
        var userId = UserId.New();

        // Pass null for the date to trigger the fallback logic
        var domainEvent = UserHelper.CreateSuspensionEvent(userId, null);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // ✅ Verify the null-coalescing operator successfully injected "Indefinite"
        await _emailService.Received(1).SendEmailAsync(
            Arg.Is<EmailRequest>(req => 
                req.To == domainEvent.Email && 
                req.Body.Contains("Estimated return: Indefinite")),
            Arg.Any<CancellationToken>());
    }
}
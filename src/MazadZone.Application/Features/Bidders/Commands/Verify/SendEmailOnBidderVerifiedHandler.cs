using MazadZone.Domain.Bidders.Events;
using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Bidders.Commands.Verify;

public class SendEmailOnBidderVerifiedHandler : INotificationHandler<BidderVerifiedDomainEvent>
{
    private readonly IEmailService _emailService;
    private readonly IBidderQueries _bidderQueries;
    private readonly ILogger<SendEmailOnBidderVerifiedHandler> _logger;

    public SendEmailOnBidderVerifiedHandler(
        IEmailService emailService,
        IBidderQueries bidderQueries,
        ILogger<SendEmailOnBidderVerifiedHandler> logger
    )
    {
        _emailService = emailService;
        _bidderQueries = bidderQueries;
        _logger = logger;
    }

    public async Task Handle(BidderVerifiedDomainEvent notification, CancellationToken ct)
    {
        // 1. Fetch the bidder to get their email address

        var bidder = await _bidderQueries.GetBidderProfile(notification.BidderId, ct);
        if (bidder is null)
        {
            GlobalLogs.LogBidderNotFound(_logger, notification.BidderId);
            return;
        }

        // 2. Prepare the email payload
        var subject = "Account Verified - Welcome to MazadZone!";
        
        // Formatted with HTML tags since your EmailRequest defaults to IsHtml = true
        var body = $"<p>Hello <strong>{bidder.FullName}</strong>,</p><p>Your account has been successfully verified. You can now start participating in auctions.</p>";

        var emailRequest = new EmailRequest(bidder.Email, subject, body, IsHtml: true);

        // 3. Send the email using the EmailRequest object
        await _emailService.SendEmailAsync(emailRequest, ct);
        
        _logger.LogInformation("Verification email sent to {Email} for Bidder {BidderId}", bidder.Email, notification.BidderId);
    }
}
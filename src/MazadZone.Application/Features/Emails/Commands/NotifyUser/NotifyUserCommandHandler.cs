using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Services;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Emails.Commands.NotifyUser;

public  sealed class NotifyUserCommandHandler : IRequestHandler<NotifyUserCommand, Result<Unit>>
{
    private readonly IEmailService _emailService;
    private readonly IUserQueries _userQueries;
    private readonly ILogger<NotifyUserCommandHandler> _logger;

    public NotifyUserCommandHandler(IEmailService emailService, IUserQueries userQueries, ILogger<NotifyUserCommandHandler> logger)
    {
        _emailService = emailService;
        _userQueries = userQueries;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(NotifyUserCommand request, CancellationToken cancellationToken)
    {
        var userEmail = await _userQueries.GetEmailByIdAsync(request.UserId.Value, cancellationToken);

        if (string.IsNullOrWhiteSpace(userEmail.Value))
        {
            GlobalLogs.LogUserNotFound(_logger, request.UserId);
            return UserErrors.NotFound;
        }

        // 3. Construct the email request using the Title and Message
        var emailRequest = new EmailRequest(
            To: userEmail.Value,
            Subject: request.Title, // The title injected from the endpoint
            Body: request.Message,
            IsHtml: true 
        );

        // 4. Send the email
        await _emailService.SendEmailAsync(emailRequest, cancellationToken);

        // 5. Return success
        return Result.Success(Unit.Value);
    }
}
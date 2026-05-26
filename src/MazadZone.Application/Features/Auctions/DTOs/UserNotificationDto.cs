using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Auctions.DTOs;

public record UserNotificationDto
(   Guid userId,
    string method,
    string title,
    string Message
);
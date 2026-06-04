namespace MazadZone.Api.Contracts.Notifications;

public record GetNotificationsRequest(
    int PageNumber = 1,
    int PageSize = 10);
namespace MazadZone.Api.Contracts.Notifications;

// UserId is intentionally excluded — it is bound from the authenticated user's JWT claims.
public record GetNotificationsRequest(
    int PageNumber = 1,
    int PageSize = 10);
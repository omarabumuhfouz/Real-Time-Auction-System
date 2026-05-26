namespace MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;
public sealed record UnverifiedSellerSummaryResponse(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    DateTime  JoinedOn);

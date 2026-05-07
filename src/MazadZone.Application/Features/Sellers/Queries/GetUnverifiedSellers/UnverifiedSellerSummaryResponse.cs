namespace MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;
public sealed record UnverifiedSellerSummaryResponse(
    Guid SellerId,
    string BankAccountNumber,
    string? TaxIdentificationNumber,
    DateTime JoinedOnUtc);
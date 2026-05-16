using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Queries.GetPrivateDetails;
public record PrivateSellerDetailsResponse(
    SellerId SellerId,
    string BankAccountNumber,
    string? TaxIdentificationNumber,
    bool IsVerified,
    decimal Rating);
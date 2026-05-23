using MazadZone.Application.Common.Messaging;
using MazadZone.Application.Features.Auctions.Queries;
using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.Queries.GetSimilarAuctions;

public sealed record GetSimilarAuctionsQuery(AuctionId AuctionId, int Limit = 6) : ICachedQuery<IReadOnlyList<AuctionsListDto>>
{
    public string CacheKey => $"similar-auctions:{AuctionId.Value}:limit={Limit}";

    public string[] Tags => ["auctions"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}

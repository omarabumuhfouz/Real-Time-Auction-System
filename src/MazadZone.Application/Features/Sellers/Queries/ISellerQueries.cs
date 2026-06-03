using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;
using MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;

using MazadZone.Domain.Sellers;
using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Sellers.Queries;

/// <summary>
/// Abstraction for Seller read models to isolate Dapper SQL from Handlers.
/// </summary>
public interface ISellerQueries : IScopedService
{
    Task<PublicSellerProfileResponse?> GetSellerProfileSummaryAsync(UserId sellerId, CancellationToken ct);
    Task<IReadOnlyList<UnverifiedSellerSummaryResponse>?> GetUnverifiedSellersAsync(CancellationToken cancellationToken);
    Task<PagedList<FeedbackDto>> GetSellerFeedbacksAsync(UserId sellerId, int page, int pageSize, CancellationToken ct);
}
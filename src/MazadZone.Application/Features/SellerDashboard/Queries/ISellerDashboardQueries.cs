using MazadZone.Application.Features.SellerDashboard.DTOs;
using MazadZone.Domain.Users.ValueObjects;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Features.SellerDashboard.Queries;

public interface ISellerDashboardQueries : IScopedService
{
    Task<SellerAuctionsResponse?> GetSellerAuctionsAsync(UserId sellerId, SellerDashboardFilter filter, CancellationToken cancellationToken);
    Task<SellerOrdersResponse?> GetSellerOrdersAsync(UserId sellerId, SellerDashboardFilter filter, CancellationToken cancellationToken);
    Task<SellerFinancialsResponse?> GetSellerFinancialsAsync(UserId sellerId, SellerDashboardFilter filter, CancellationToken cancellationToken);
}

using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Users.DTOs;
using MazadZone.Application.Features.Users.Queries.GetProfileSettings;
using MazadZone.Application.Features.Users.Queries.GetPaymentMethods;
using MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;
using MazadZone.Application.Features.Users.Queries.GetUserTrustStats;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public interface IUserQueries : IScopedService
{
    //Task<Result<UserDto>> GetUserByIdAsync(UserId userId, CancellationToken cancellationToken);
   // Task<Result<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<Result<Address>> GetAddressByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<Email>> GetEmailByIdAsync(Guid userId, CancellationToken cancellationToken); 
    Task<ProfileSettingsResponse?> GetProfileSettings(UserId userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentMethodResponse>> GetPaymentMethodsAsync(UserId userId, CancellationToken ct);
    Task<PagedList<UserDto>?> GetUsersAsync(UserFilterParams filter, CancellationToken ct);
    Task<IReadOnlyList<UserDto>> ExportSelectedUsersAsync(IEnumerable<Guid> userIds, CancellationToken ct);
    Task<RawUserTrustMetrics> GetUserTrustMetricsAsync(
        DateTime currStart, DateTime currEnd,
        DateTime prevStart, DateTime prevEnd,
        CancellationToken ct);

    Task<UserGrowthDataResult> GetUserGrowthTrendsAsync(
        DateTime currStart, DateTime currEnd,
        DateTime prevStart, DateTime prevEnd,
        CancellationToken ct);



}
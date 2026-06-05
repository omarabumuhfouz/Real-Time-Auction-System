using MazadZone.Application.Services;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Users.Queries.GetPaymentMethods;

public class GetPaymentMethodsQueryHandler : IQueryHandler<GetPaymentMethodsQuery, IReadOnlyList<PaymentMethodResponse>>
{
    private readonly IUserQueries _userQueries;
    private readonly ILogger<GetPaymentMethodsQueryHandler> _logger;

    public GetPaymentMethodsQueryHandler(IUserQueries userQueries, ILogger<GetPaymentMethodsQueryHandler> logger)
    {
        _userQueries = userQueries;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<PaymentMethodResponse>>> Handle(GetPaymentMethodsQuery request, CancellationToken ct)
    {
        var profile = await _userQueries.GetProfileSettings(request.UserId, ct);
        if (profile is null)
        {
            GlobalLogs.LogUserNotFound(_logger, request.UserId);
            return UserErrors.NotFound;
        }

        var paymentMethods = await _userQueries.GetPaymentMethodsAsync(request.UserId, ct);
        return Result.Success(paymentMethods);
    }
}

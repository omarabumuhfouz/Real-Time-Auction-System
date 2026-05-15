using MazadZone.Application.Services;
using MazadZone.Domain.Payments.Errors;

namespace MazadZone.Application.Features.Payments.Commands.UnauthorizeOutbidPayments;

public class UnauthorizeOutbidPaymentsCommandHandler : ICommandHandler<UnauthorizeOutbidPaymentsCommand, Unit>
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<UnauthorizeOutbidPaymentsCommandHandler> _logger;

    public UnauthorizeOutbidPaymentsCommandHandler(
        IPaymentService paymentService,
        ILogger<UnauthorizeOutbidPaymentsCommandHandler> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UnauthorizeOutbidPaymentsCommand request, CancellationToken ct)
    {
        UnauthorizeOutbidPaymentsLogs.LogAttempt(_logger, request.OutbidGatewayAuthHoldIds.Count);

        try
        {
            // Release the holded amount for all the outbid bidders
            foreach (var gatewayAuthHoldId in request.OutbidGatewayAuthHoldIds)
            {
                await _paymentService.UnAuthorizeAsync(gatewayAuthHoldId, ct);
            }

            UnauthorizeOutbidPaymentsLogs.LogSuccess(_logger, request.OutbidGatewayAuthHoldIds.Count);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            UnauthorizeOutbidPaymentsLogs.LogFailure(_logger, request.OutbidGatewayAuthHoldIds.Count, ex.Message);
            return PaymentErrors.PaymentUnAuthorizationFailed;
        }
    }
}

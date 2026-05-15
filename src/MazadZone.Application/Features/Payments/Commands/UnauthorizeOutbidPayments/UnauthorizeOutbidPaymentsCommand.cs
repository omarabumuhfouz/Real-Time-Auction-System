using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Payments.Commands.UnauthorizeOutbidPayments;

public record UnauthorizeOutbidPaymentsCommand(
    List<string> OutbidGatewayAuthHoldIds
) : ICommand<Unit>;

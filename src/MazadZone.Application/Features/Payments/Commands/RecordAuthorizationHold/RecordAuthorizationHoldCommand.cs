using MzadZone.Domain.Payments;
using MzadZone.Domain.Payments.Entities;

namespace MazadZone.Application.Features.Payments.Commands.RecordAuthorizationHold;

public record RecordAuthorizationHoldCommand(
    Payment Payment,
    string GatewayAuthHoldId
) : ICommand<Unit>;

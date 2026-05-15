using MzadZone.Domain.Payments;
using MzadZone.Domain.Payments.Entities;

namespace MazadZone.Application.Features.Payments.Commands.CaptureAuthorizedHold;

public record CaptureAuthorizedHoldCommand(
    Payment Payment,
    string GatewayAuthHoldId
) : ICommand<Unit>;

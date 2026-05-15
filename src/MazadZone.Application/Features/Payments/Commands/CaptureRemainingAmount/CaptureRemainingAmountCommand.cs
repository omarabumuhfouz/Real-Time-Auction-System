using MazadZone.Domain.Entities.Orders;

namespace MazadZone.Application.Features.Payments.Commands.CaptureRemainingAmount;

public record CaptureRemainingAmountCommand(
    OrderId OrderId
) : ICommand<Unit>;

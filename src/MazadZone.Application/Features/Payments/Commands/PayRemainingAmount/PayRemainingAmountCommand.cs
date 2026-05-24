using MediatR;
using System;
using MazadZone.Application.Common;

namespace MazadZone.Application.Features.Payments.Commands.PayRemainingAmount;

public record PayRemainingAmountCommand(Guid OrderId, string PaymentMethodId) : IRequest<Result<Unit>>;
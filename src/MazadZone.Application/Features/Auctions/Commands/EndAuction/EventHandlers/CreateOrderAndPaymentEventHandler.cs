using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using MazadZone.Domain.Auctions.Events;
using MazadZone.Domain.Orders;
using MzadZone.Domain.Payments;
using MzadZone.Domain.Payments.Entities;
using MazadZone.Domain.Repositories;
using MazadZone.Application.Services;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Auctions.EventHandlers;

public class CreateOrderAndPaymentEventHandler(
    IOrderRepository _orderRepository, 
    IPaymentRepository _paymentRepository,
    IAuctionRepository _auctionRepository,
    IOrderJobScheduler _orderJobScheduler,
    IUnitOfWork _unitOfWork,
    ILogger<CreateOrderAndPaymentEventHandler> _logger,
    IUserQueries _userQueries
) : INotificationHandler<AuctionEndedDomainEvent>
{
    // create order and payment the auth holded 
    public async Task Handle(AuctionEndedDomainEvent notification, CancellationToken cancellationToken)
    {
        // 1. retreve auction
        var auction = await _auctionRepository.GetByIdAsync(notification.AuctionId, cancellationToken);
        if (auction is null || auction.CurrentLeadingBid is null) return;

        var winningBid = auction.CurrentLeadingBid;
        var address = await _userQueries.GetAddressByIdAsync(winningBid.BidderId.Value, cancellationToken);
        if (address is null) return;

        // 2. Create auction
        var orderResult = Order.Create(
            auctionId: auction.Id,
            bidderId: winningBid.BidderId,
            winningBidId: winningBid.Id,
            totalAmount: winningBid.Amount.Amount,
            receiptAddress: address.Value
        );
        if (orderResult.IsFailure)
        {
            _logger.LogError("Failed to create order: {Error}", orderResult.TopError);
            return;
        }

        var order = orderResult.Value;
        _orderRepository.Add(order);

        // 3. payment  
        var paymentResult = Payment.Create(order.Id, UserId.From(winningBid.BidderId.Value), winningBid.DepositAmount);
        if (paymentResult.IsFailure)
        {
            _logger.LogError("Failed to initialize payment ledger.");
            return; 
        }
        var payment = paymentResult.Value;

        // 4. record in domain
        payment.RecordTransactionAttempt(winningBid.GatewayAuthHoldId, TransactionType.AuthorizationHold);
        payment.ResolveTransactionSuccess(winningBid.GatewayAuthHoldId);

        // 5.(Capture) no capture in this phase (we capture it in CancelPayment)

        //var captureTransactionId = await _paymentService.CaptureHoldedAmountAsync(winningBid.GatewayAuthHoldId, cancellationToken);
        // if (string.IsNullOrEmpty(captureTransactionId))
        // {
        //     _logger.LogError("Gateway failure: Could not capture hold {HoldId}", winningBid.GatewayAuthHoldId);
     
        // }
        // else
        // {
           
        //     payment.RecordTransactionAttempt(captureTransactionId, TransactionType.DepositCaptured);
        //     payment.ResolveTransactionSuccess(captureTransactionId);
        // }

        _paymentRepository.Add(payment);

        _orderJobScheduler.ScheduleUnpaidOrderCancellation(order.Id.Value, DateTimeOffset.UtcNow.AddMinutes(OrderConstants.UnpaidOrderCancellationMinutes));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
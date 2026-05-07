using AutoMapper;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Application.Features.Orders.Commands.CreateOrder;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Entities.Users;

namespace MazadZone.Api.Contracts.Orders;

public record CreateOrderRequest(
    BidderId BidderId,
    BidId WinningBidId,
    AddressDto ReceiptAddress,
    decimal Amount,
    string DepositCaptureTransactionId)
{
        public CreateOrderCommand ToCommand(IMapper mapper) => new(
            BidderId,
            WinningBidId,
            mapper.Map<Address>(ReceiptAddress),
            Amount,
            DepositCaptureTransactionId);
    }

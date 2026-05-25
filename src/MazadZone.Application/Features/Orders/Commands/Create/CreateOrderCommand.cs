using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Orders.Commands.Create;

public record CreateOrderCommand(
    AuctionId AuctionId,
    UserId BidderId,
    BidId WinningBidId,
    AddressDto ReceiptAddress,
    decimal Amount) : ICommand<OrderId>;
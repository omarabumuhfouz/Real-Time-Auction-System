using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.Commands.PlaceBid;

public sealed record PlaceBidCommand(
    AuctionId AuctionId,
    UserId BidderId,
    string PaymentMethodId,
    Money Amount) : ICommand<Unit>;
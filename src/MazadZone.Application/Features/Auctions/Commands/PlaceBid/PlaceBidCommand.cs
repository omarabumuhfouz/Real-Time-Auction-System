using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users;
using MazadZone.Domain.ValueObjects;

namespace MazadZone.Application.Features.Auctions.Commands.PlaceBid;

public sealed record PlaceBidCommand(
    AuctionId AuctionId,
    BidderId BidderId,
    Money Amount) : ICommand<Unit>;
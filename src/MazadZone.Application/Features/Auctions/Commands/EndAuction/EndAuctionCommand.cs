using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.Commands.EndAuction;

public sealed record EndAuctionCommand(AuctionId AuctionId) : ICommand<Unit>;
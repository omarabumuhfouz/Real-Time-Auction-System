using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.Commands.CancelAuction;

public sealed record CancelAuctionCommand(AuctionId AuctionId) : ICommand<Unit>;
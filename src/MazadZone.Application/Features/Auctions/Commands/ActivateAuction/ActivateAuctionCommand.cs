using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.Commands.ActivateAuction;

public sealed record ActivateAuctionCommand(AuctionId AuctionId) : ICommand<Unit>;
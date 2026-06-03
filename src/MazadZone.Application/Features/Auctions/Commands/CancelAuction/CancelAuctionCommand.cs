using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Auctions.Commands.CancelAuction;

// SellerId is included so the handler can verify the requesting user owns this auction.
public sealed record CancelAuctionCommand(AuctionId AuctionId, string Reason, UserId SellerId) : ICommand<Unit>;
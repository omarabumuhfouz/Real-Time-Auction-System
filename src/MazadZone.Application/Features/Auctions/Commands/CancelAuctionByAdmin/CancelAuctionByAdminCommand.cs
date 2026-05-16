using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.Commands.CancelAuctionByAdmin;

public sealed record CancelAuctionByAdminCommand(AuctionId AuctionId) 
: ICommand<Unit>;
namespace MazadZone.Application.Features.Users.Commands.Ban.Models;

public record AffectedAuctionDto(Guid Id, string Title, Guid SellerId, HashSet<Guid> OtherBidderIds);
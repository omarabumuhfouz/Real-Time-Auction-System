using MazadZone.Application.Features.Auctions.DTOs;

namespace MazadZone.Application.Features.Disputes.Commands.OpenDispute;

public record OpenDisputeCommand(OrderId OrderId,DisputeTypeId DisputeTypeId, string Title, string Description, List<ImageModelDto>? Images = null) : ICommand<Guid>;
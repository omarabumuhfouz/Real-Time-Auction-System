using MazadZone.Domain.Bidders;

namespace MazadZone.Application.Features.Bidders.Commands.Verify;
public record VerifyBidderCommand(BidderId BidderId) : ICommand<Unit>;
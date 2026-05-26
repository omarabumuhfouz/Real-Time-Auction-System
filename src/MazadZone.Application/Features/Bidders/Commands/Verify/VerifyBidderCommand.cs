using MazadZone.Domain.Bidders;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Bidders.Commands.Verify;
public record VerifyBidderCommand(UserId BidderId) : ICommand<Unit>;
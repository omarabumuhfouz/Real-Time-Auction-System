using FluentValidation;

namespace MazadZone.Application.Features.Auctions.Commands.EndAuction;

public sealed class EndAuctionValidator : AbstractValidator<EndAuctionCommand>
{
    public EndAuctionValidator()
    {
        RuleFor(x => x.AuctionId)
            .NotEmpty();
    }
}

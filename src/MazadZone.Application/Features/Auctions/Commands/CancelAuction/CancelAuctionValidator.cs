using FluentValidation;

namespace MazadZone.Application.Features.Auctions.Commands.CancelAuction;

public sealed class CancelAuctionValidator : AbstractValidator<CancelAuctionCommand>
{
    public CancelAuctionValidator()
    {
        RuleFor(x => x.AuctionId)
            .NotEmpty();
    }
}

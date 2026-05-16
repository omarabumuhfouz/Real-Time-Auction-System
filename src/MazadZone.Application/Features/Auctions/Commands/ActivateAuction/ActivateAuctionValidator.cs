using FluentValidation;
using MazadZone.Application.Features.Auctions.Commands.ActivateAuction;

namespace MazadZone.Application.Features.Auctions.Commands.ActivateAuction;

public sealed class ActivateAuctionValidator : AbstractValidator<ActivateAuctionCommand>
{
    public ActivateAuctionValidator()
    {
        RuleFor(x => x.AuctionId)
            .NotEmpty();
    }
}

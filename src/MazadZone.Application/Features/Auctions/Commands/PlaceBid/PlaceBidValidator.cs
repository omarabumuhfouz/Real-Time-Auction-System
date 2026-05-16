using FluentValidation;

namespace MazadZone.Application.Features.Auctions.Commands.PlaceBid;

public sealed class PlaceBidValidator : AbstractValidator<PlaceBidCommand>
{
    public PlaceBidValidator()
    {
        RuleFor(x => x.AuctionId)
            .NotEmpty();

        RuleFor(x => x.BidderId)
            .NotEmpty();

        RuleFor(x => x.Amount.Amount)
            .GreaterThan(0m)
            .WithMessage("Bid amount must be greater than zero.");
    }
}

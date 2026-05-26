using FluentValidation;

namespace MazadZone.Application.Features.Auctions.Queries.GetSimilarAuctions;

public sealed class GetSimilarAuctionsValidator : AbstractValidator<GetSimilarAuctionsQuery>
{
    public GetSimilarAuctionsValidator()
    {
        RuleFor(x => x.AuctionId)
            .NotEmpty();

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 20)
            .WithMessage("Limit must be between 1 and 20.");
    }
}

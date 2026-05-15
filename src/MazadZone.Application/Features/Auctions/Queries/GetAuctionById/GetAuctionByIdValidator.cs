using FluentValidation;

namespace MazadZone.Application.Features.Auctions.Queries.GetAuctionById;

public sealed class GetAuctionByIdValidator : AbstractValidator<GetAuctionByIdQuery>
{
    public GetAuctionByIdValidator()
    {
        RuleFor(x => x.AuctionId)
            .NotEmpty();
    }
}

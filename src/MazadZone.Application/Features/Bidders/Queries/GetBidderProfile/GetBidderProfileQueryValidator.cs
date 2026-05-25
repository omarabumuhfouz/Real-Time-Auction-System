using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;

public class GetBidderProfileQueryValidator : AbstractValidator<GetBidderProfileQuery>
{
    public GetBidderProfileQueryValidator()
    {
        RuleFor(x => x.BidderId).MustBeValidUserId();
    }
}
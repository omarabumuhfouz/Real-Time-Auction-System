using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;

public sealed class GetPublicSellerProfileValidator : AbstractValidator<GetPublicSellerProfileQuery>
{
    public GetPublicSellerProfileValidator()
    {
        RuleFor(x => x.SellerId).MustBeValidUserId();
    }
}
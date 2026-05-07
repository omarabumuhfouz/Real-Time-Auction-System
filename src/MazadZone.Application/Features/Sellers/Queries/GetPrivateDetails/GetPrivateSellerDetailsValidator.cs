using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Sellers.Queries.GetPrivateDetails;

public sealed class GetPrivateSellerDetailsValidator : AbstractValidator<GetPrivateSellerDetailsQuery>
{
    public GetPrivateSellerDetailsValidator()
    {
        RuleFor(x => x.SellerId).MustBeValidSellerId();
    }
}
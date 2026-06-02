namespace MazadZone.Application.Features.Sellers.Queries.GetFeedbacks;
public class GetSellerFeedbacksQueryValidator : AbstractValidator<GetSellerFeedbacksQuery>
{
    public GetSellerFeedbacksQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(50); // Prevent massive payloads
    }
}
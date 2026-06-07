namespace MazadZone.Application.Features.Orders.Queries.GetBidderWonOrders;

public class GetBidderWonOrdersQueryValidator : AbstractValidator<GetBidderWonOrdersQuery>
{
    public GetBidderWonOrdersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");
            
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

        // Validates that the string is a real OrderStatus enum if it is provided
        RuleFor(x => x.Status)
            .IsEnumName(typeof(OrderStatus), caseSensitive: false)
            .When(x => !string.IsNullOrWhiteSpace(x.Status) && !x.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Invalid order status filter provided.");
    }
}
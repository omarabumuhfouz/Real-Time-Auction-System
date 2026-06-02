namespace MazadZone.Application.Features.Orders.Queries.GetSellerOrders;

public class GetSellerOrdersQueryValidator : AbstractValidator<GetSellerOrdersQuery>
{
    public GetSellerOrdersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");
            
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

        // Validate that the string matches the OrderStatus enum (ignoring case)
        // We skip this check if the frontend sends nothing, null, or explicitly "All"
        RuleFor(x => x.Status)
    .IsEnumName(typeof(OrderStatus), caseSensitive: false)
    // Fix: Explicitly use x.Status instead of just x
    .When(x => !string.IsNullOrWhiteSpace(x.Status) && !x.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
    .WithMessage("Invalid order status filter provided.");
    }
}
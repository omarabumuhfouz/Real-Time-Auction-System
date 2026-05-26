namespace MazadZone.Application.Features.Disputes.Queries.GetFiltered;

using FluentValidation;
using System.Linq;

public class GetFilteredDisputesQueryValidator : AbstractValidator<GetFilteredDisputesQuery>
{
    public GetFilteredDisputesQueryValidator()
    {
        // 1. Date Validation
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("The 'From Date' must be earlier than or equal to the 'To Date'.");

        // 2. Sort Column Validation (Preventing random strings)
        var allowedSortColumns = new[] { "submitteddate", "category", "status", "biddername", "sellername" };
        
        RuleFor(x => x.SortColumn)
            .Must(x => string.IsNullOrWhiteSpace(x) || allowedSortColumns.Contains(x.ToLower()))
            .WithMessage("Invalid sort column provided.");
    }
}
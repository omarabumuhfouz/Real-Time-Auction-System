namespace MazadZone.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.FilterParams.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.FilterParams.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be at least 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100.");

        // Optional: Ensure SearchTerm isn't excessively long
        RuleFor(x => x.FilterParams.SearchTerm)
            .MaximumLength(100)
            .WithMessage("Search term is too long.");

RuleFor(x => x.FilterParams.JoinedDate)
            .Must(date => string.IsNullOrWhiteSpace(date) || DateTime.TryParse(date, out _))
            .WithMessage("JoinedDate must be a valid date format.");
    }
}
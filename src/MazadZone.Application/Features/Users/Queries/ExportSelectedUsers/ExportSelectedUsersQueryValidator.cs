namespace MazadZone.Application.Features.Users.Queries.ExportSelectedUsers;

public class ExportSelectedUsersQueryValidator : AbstractValidator<ExportSelectedUsersQuery>
{
    public ExportSelectedUsersQueryValidator()
    {
        RuleFor(x => x.SelectedUserIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("At least one user ID must be provided for export.");
    }
}
using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Users.Queries.GetProfileSettings;

public class GetProfileSettingsQueryValidator : AbstractValidator<GetProfileSettingsQuery>
{
    public GetProfileSettingsQueryValidator()
    {
        RuleFor(x => x.UserId).MustBeValidUserId();
    }
}
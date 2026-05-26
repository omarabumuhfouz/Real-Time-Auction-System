using MazadZone.Application.Features.Bidders.DTOs;

namespace MazadZone.Application.Features.Users.Queries.GetProfileSettings;

public record ProfileSettingsResponse(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string NationalId,
    string City,
    string Street,
    string Building,
    string Landmark
)
{
    public static ProfileSettingsResponse Empty => new ProfileSettingsResponse(Guid.Empty, "", "", "", "","", "", "", "");
}
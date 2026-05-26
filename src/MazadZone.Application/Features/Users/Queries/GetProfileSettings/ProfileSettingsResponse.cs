using MazadZone.Application.Features.Bidders.DTOs;

namespace MazadZone.Application.Features.Users.Queries.GetProfileSettings;

public record ProfileSettingsResponse(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string NationalId,
    List<AddressDto> Addresses
)
{
    public static ProfileSettingsResponse Empty => new ProfileSettingsResponse(Guid.Empty, "", "", "", "", new List<AddressDto>());
}
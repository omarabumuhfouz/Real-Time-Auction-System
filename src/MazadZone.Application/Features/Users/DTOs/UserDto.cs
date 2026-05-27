namespace MazadZone.Application.Features.Users.DTOs;

public record UserDto
(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Role,
    string Status,
    DateTime JoinedAt,
    DateTime LastLogin
);
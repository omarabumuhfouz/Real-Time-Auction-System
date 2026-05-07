namespace MazadZone.Application.Features.Users.DTOs;

public record UserDto
(
    string Email,
    string Password,
    string PhoneNumber,
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName
);
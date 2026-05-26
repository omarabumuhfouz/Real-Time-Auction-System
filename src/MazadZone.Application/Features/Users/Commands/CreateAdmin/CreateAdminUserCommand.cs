namespace MazadZone.Application.Users.Commands.CreateAdminUser;

public record CreateAdminUserCommand(
    string Email,
    string Password, 
    string PhoneNumber,
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName
) : ICommand<Guid>;
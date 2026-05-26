using MazadZone.Application.Services;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Users.Commands.CreateAdminUser;

public class CreateAdminUserCommandHandler : ICommandHandler<CreateAdminUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService; // Assuming you have an abstraction for hashing
    private readonly IUnitOfWork _unitOfWork;

    public CreateAdminUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordService passwordService, 
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAdminUserCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if(emailResult.IsFailure) return emailResult.TopError;
        
        bool emailExists = await _userRepository.IsEmailInUseAsync(emailResult.Value, cancellationToken);
        if (emailExists)
        {
            return EmailErrors.AlreadyInUse;
        }

        // 2. Hash the raw password
        string hashedPassword = _passwordService.HashPassword(request.Password);

        // 3. Prepare roles (Explicitly provisioning Admin)
        var roles = new HashSet<UserRole> { UserRole.Admin }; // Ensure UserRole.Admin exists in your enum/smart enum

        // 4. Invoke Domain Factory Method
        var userResult = User.Create(
            request.Email,
            hashedPassword,
            request.PhoneNumber,
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            roles
        );

        if (userResult.IsFailure)
        {
            return userResult.TopError;
        }

        User user = userResult.Value;
        _userRepository.Add(user);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Return the strongly-typed aggregate root ID value
        return user.Id.Value;
    }
}
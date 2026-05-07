using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Users.Commands.Suspend;
public class SuspendUserCommandHandler : ICommandHandler<SuspendUserCommand, Unit>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SuspendUserCommandHandler> _logger;

    public SuspendUserCommandHandler(
        IUserRepository userRepo,
        IUnitOfWork unitOfWork,
        ILogger<SuspendUserCommandHandler> logger
    )
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(SuspendUserCommand request, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId.Value, ct);
        if (user is null)
        {
            _logger.LogUserNotFound(UserErrorCodes.NotFound, request.UserId);
            return UserErrors.NotFound;
        }

        var result = user.Suspend(request.Until);
        if (result.IsFailure)
        {
            _logger.LogSuspensionDomainError(request.UserId, result.TopError);
            return result.TopError;
        }

        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogUserSuspended(user.Id, request.Until);
        return Unit.Value;
    }
}
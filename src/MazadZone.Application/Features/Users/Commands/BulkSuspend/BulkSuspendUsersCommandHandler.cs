using MazadZone.Application.Features.Users.Commands.Suspend;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Users.Commands.BulkSuspend;

public class BulkSuspendUsersCommandHandler : ICommandHandler<BulkSuspendUsersCommand, Unit>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkSuspendUsersCommandHandler> _logger;

    public BulkSuspendUsersCommandHandler(
        IUserRepository userRepo,
        IUnitOfWork unitOfWork,
        ILogger<BulkSuspendUsersCommandHandler> logger)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(BulkSuspendUsersCommand request, CancellationToken ct)
    {
        //  Create the reason once for all users
        var reasonResult = Reason.Create(request.Reason);
        if (reasonResult.IsFailure) return reasonResult.TopError;

        //  Fetch all requested users in a single database round-trip
        var users = await _userRepo.GetByIdsAsync(request.UserIds, ct);

        if (!users.Any())
        {
            _logger.LogWarning("Bulk Suspend failed: No users found for the provided IDs.");
            return UserErrors.NotFound;
        }

        //  Process Domain Logic using tracked entities
        foreach (var user in users)
        {
            var result = user.Suspend(reasonResult.Value, request.Until);
            
            if (result.IsFailure)
            {
                SuspendUserLogs.LogDomainViolation(_logger, user.Id, result.TopError.Code);
                return result.TopError; 
            }
        }

        //  Commit the transaction
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Successfully bulk-suspended {Count} users until {Until} for reason: {Reason}", users.Count, request.Until, request.Reason);

        return Unit.Value;
    }
}
using MediatR;
using Microsoft.Extensions.Logging;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Errors;
using MazadZone.Application.Features.Users.Commands.Activate;

namespace MazadZone.Application.Features.Users.Commands.BulkActivate;

public class BulkActivateUsersCommandHandler : ICommandHandler<BulkActivateUsersCommand, Unit>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkActivateUsersCommandHandler> _logger;

    public BulkActivateUsersCommandHandler(
        IUserRepository userRepo,
        IUnitOfWork unitOfWork,
        ILogger<BulkActivateUsersCommandHandler> logger)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(BulkActivateUsersCommand request, CancellationToken ct)
    {
        // 1. Fetch all requested users in a single database round-trip
        var users = await _userRepo.GetByIdsAsync(request.UserIds, ct);

        if (!users.Any())
        {
            _logger.LogWarning("Bulk Activate failed: No users found for provided IDs.");
            return UserErrors.NotFound;
        }

        // 2. Process Domain Logic using tracked entities
        foreach (var user in users)
        {
            var result = user.Activate();
            
            if (result.IsFailure)
            {
                // Strict All-or-Nothing transaction. 
                // Aborts immediately if ANY user fails domain validation.
                ActivateUserLogs.LogDomainViolation(_logger, user.Id, result.TopError.Code);
                return result.TopError; 
            }
        }

        // 3. Commit the transaction
        // NOTE: We DO NOT call _userRepo.Update() here. EF Core's Change Tracker 
        // automatically detected the state changes from user.Activate().
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Successfully bulk-activated {Count} users.", users.Count);

        return Unit.Value;
    }
}
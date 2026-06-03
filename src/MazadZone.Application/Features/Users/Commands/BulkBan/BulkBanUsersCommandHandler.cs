using MazadZone.Application.Features.Users.Commands.Ban;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Users.Commands.BulkBan;

public class BulkBanUsersCommandHandler : ICommandHandler<BulkBanUsersCommand, Unit>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkBanUsersCommandHandler> _logger;

    public BulkBanUsersCommandHandler(
        IUserRepository userRepo,
        IUnitOfWork unitOfWork,
        ILogger<BulkBanUsersCommandHandler> logger)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(BulkBanUsersCommand request, CancellationToken ct)
    {
        // Create the reason once for all users
        var reasonResult = Reason.Create(request.Reason);
        if (reasonResult.IsFailure) return reasonResult.TopError;

        //  Fetch all requested users in a single database round-trip
        var users = await _userRepo.GetByIdsAsync(request.UserIds, ct);

        if (!users.Any())
        {
            _logger.LogWarning("Bulk Ban failed: No users found for the provided IDs.");
            return UserErrors.NotFound;
        }

        //  Process Domain Logic using tracked entities
        foreach (var user in users)
        {
            var result = user.Ban(reasonResult.Value);

            if (result.IsFailure)
            {
                BanUserLogs.LogDomainViolation(_logger, user.Id, result.TopError.Code);
                return result.TopError;
            }
        }

        // Commite Changes
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Successfully bulk-banned {Count} users for reason: {Reason}", users.Count, request.Reason);

        return Unit.Value;
    }
}
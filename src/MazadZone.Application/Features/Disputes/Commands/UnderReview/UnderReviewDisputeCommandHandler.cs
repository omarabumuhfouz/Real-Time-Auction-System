using MazadZone.Application.Features.Disputes;
using MazadZone.Domain.Disputes;

namespace MazadZone.Application.Features.Orders.Commands.ResolveDispute;

public class UnderReviewDisputeCommandHandler : ICommandHandler<UnderReviewDisputeCommand, Unit>
{
    private readonly IDisputeRepository _disputeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnderReviewDisputeCommandHandler> _logger;

    public UnderReviewDisputeCommandHandler(IDisputeRepository disputeRepository, IUnitOfWork unitOfWork, ILogger<UnderReviewDisputeCommandHandler> logger)
    {
        _disputeRepository = disputeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UnderReviewDisputeCommand request, CancellationToken ct)
    {
        var dispute = await _disputeRepository.GetByIdAsync(request.DisputeId, ct);
        if (dispute is null)
        {
            DisputeLogs.LogDisputeNotFound(_logger, request.DisputeId);
            return DisputeErrors.NotFound;
        }

        var result = dispute.UnderReview();
        if (result.IsFailure)
        {
            return result.TopError;
            //Log later
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }

}
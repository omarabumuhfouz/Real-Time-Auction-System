using MazadZone.Domain.Bidders;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Bidders.Commands.Verify;

public class VerifyBidderCommandHandler : ICommandHandler<VerifyBidderCommand, Unit>
{
    private readonly IBidderRepository _bidderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VerifyBidderCommandHandler> _logger;

    public VerifyBidderCommandHandler(IBidderRepository bidderRepository, IUnitOfWork unitOfWork, ILogger<VerifyBidderCommandHandler> logger)
    {
        _bidderRepository = bidderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(VerifyBidderCommand request, CancellationToken cancellationToken)
    {
        var bidder = await _bidderRepository.GetByIdAsync(request.BidderId.Value, cancellationToken);
        if (bidder is null)
        {
            GlobalLogs.LogBidderNotFound(_logger, request.BidderId);
            return BidderErrors.NotFound;
        }

        bidder.Verify();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
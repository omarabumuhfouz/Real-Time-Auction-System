using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Bidders;

namespace MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;

public class GetBidderProfileQueryHandler : IQueryHandler<GetBidderProfileQuery, BidderProfileDto>
{
    private readonly IBidderQueries _bidderRepository;
    private readonly ILogger _logger;

    public GetBidderProfileQueryHandler(
        IBidderQueries bidderRepository,
        ILogger<GetBidderProfileQueryHandler> logger
    )
    {
        _bidderRepository = bidderRepository;
        _logger = logger;
    }

    public async Task<Result<BidderProfileDto>> Handle(GetBidderProfileQuery request, CancellationToken cancellationToken)
    {
        var bidderDto = await _bidderRepository.GetBidderProfile(request.BidderId);

        if (bidderDto is null)
        {
            GlobalLogs.LogBidderNotFound(_logger, request.BidderId);
            return BidderErrors.NotFound;
        }

        return bidderDto;
    }
}
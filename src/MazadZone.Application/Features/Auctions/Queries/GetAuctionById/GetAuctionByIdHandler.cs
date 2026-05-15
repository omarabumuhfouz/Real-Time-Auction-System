using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Auctions.Queries.GetAuctionById;

public class GetAuctionByIdHandler
(IAuctionQueries _auctionQueries, ILogger<GetAuctionByIdHandler> _logger
)
: IQueryHandler<GetAuctionByIdQuery, AuctionDto>
{

    public async Task<Result<AuctionDto>> Handle(GetAuctionByIdQuery request, CancellationToken ct)
    {

        var auction = await _auctionQueries.GetAuctionByIdAsync(request.AuctionId.Value, ct);

        if (auction is null)
        {
            _logger.LogAuctionNotFound(request.AuctionId.Value);
            return Result.Failure<AuctionDto>(AuctionErrors.NotFound);
        }

        _logger.LogHandlingGetAuctionById(request.AuctionId.Value);
        return Result.Success(auction);
    
    }
}
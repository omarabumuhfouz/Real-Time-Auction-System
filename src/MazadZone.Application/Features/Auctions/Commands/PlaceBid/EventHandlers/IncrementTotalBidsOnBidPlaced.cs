// namespace MazadZone.Application.Features.Auctions.EventHandlers;

// using MazadZone.Domain.Auctions.Events;
// using MazadZone.Domain.Repositories;
// using MediatR;
// using Microsoft.Extensions.Logging;

// public sealed class IncrementTotalBidsOnBidPlaced : INotificationHandler<BidPlacedDomainEvent>
// {
//     private readonly IBidderRepository _bidderRepository;
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly ILogger<IncrementTotalBidsOnBidPlaced> _logger;


//     public async Task Handle(BidPlacedDomainEvent notification, CancellationToken cancellationToken)
//     {
//         _logger.LogInformation(
//             "Processing bid point metric for Bidder: {BidderId} on Auction: {AuctionId}", 
//             notification.BidderId, 
//             notification.AuctionId);

//         // Fetch the profile aggregate wrapper tied to this UserId
//         var bidder = await _bidderRepository.GetByIdAsync(notification.BidderId, cancellationToken);

//         if (bidder is null)
//         {
//             _logger.LogWarning(
//                 "Profile metrics aggregate not found for bidding user: {BidderId}. Points update aborted.", 
//                 notification.BidderId);
//             return;
//         }

//         // Invoke the domain mutation logic safely
//         bidder.RecordPidPlaced();

//         // Persist trackable changes back to your database store
//         await _unitOfWork.SaveChangesAsync(cancellationToken);

//         _logger.LogInformation(
//             "Successfully incremented TotalPidsPlaced for User: {BidderId}.", 
//             notification.BidderId);
//     }
// }
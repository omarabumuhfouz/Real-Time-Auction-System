// namespace MazadZone.Application.Features.Auctions.EventHandlers;

// using MediatR;
// using Microsoft.Extensions.Logging;
// using MazadZone.Domain.Repositories;

// public sealed class TrackAuctionParticipationOnBidPlaced : INotificationHandler<PlacedBidDomainEvent>
// {
//     private readonly IBidderRepository _bidderRepository; 
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly ILogger<TrackAuctionParticipationOnBidPlaced> _logger;


//     public async Task Handle(PlacedBidDomainEvent notification, CancellationToken cancellationToken)
//     {
//         // 1. Check if the user has already participated in this specific auction
//         bool hasAlreadyBidOnThisAuction = await _bidRepository.HasUserBidOnAuctionAsync(
//             notification.BidderId, 
//             notification.AuctionId, 
//             cancellationToken);

//         // If they have prior bids here, their participation point is already counted.
//         if (hasAlreadyBidOnThisAuction)
//         {
//             return; 
//         }

//         _logger.LogInformation(
//             "New auction participation detected! User: {BidderId} joined Auction: {AuctionId}", 
//             notification.BidderId, 
//             notification.AuctionId);

//         // 2. Fetch the profile stats aggregate
//         var profile = await _sellerRepository.GetByIdAsync(notification.BidderId, cancellationToken);

//         if (profile is null)
//         {
//             _logger.LogWarning("Profile metrics not found for user: {BidderId}.", notification.BidderId);
//             return;
//         }

//         // 3. Increment the unique participation counter safely
//         profile.RecordAuctionParticipated();

//         // 4. Save state changes
//         await _unitOfWork.SaveChangesAsync(cancellationToken);
//     }
// }
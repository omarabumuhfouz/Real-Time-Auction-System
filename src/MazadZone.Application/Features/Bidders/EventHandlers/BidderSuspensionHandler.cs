// namespace MazadZone.Application.Features.Bidders.EventHandlers;
// public class BidderSuspensionHandler : IDomainEventHandler<BidderExceededUnpaidLimitEvent>
// {
//     private readonly IUserRepository _userRepository;
//     private readonly IUnitOfWork _unitOfWork;

//     public async Task Handle(BidderExceededUnpaidLimitEvent notification, CancellationToken ct)
//     {
//         // 1. Fetch the User (The Identity Aggregate)
//         var user = await _userRepository.GetByIdAsync(notification.UserId, ct);
        
//         if (user is not null)
//         {
//             // 2. Execute the status change inside the User aggregate
//             user.Suspend("Automatic suspension: 3 unpaid auction wins.");
            
//             // 3. Persist
//             await _unitOfWork.SaveChangesAsync(ct);
//         }
//     }
// }
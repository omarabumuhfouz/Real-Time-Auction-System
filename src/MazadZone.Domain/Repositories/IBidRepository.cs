using MazadZone.Domain.Auctions;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Domain.Repositories;


public interface IBidRepository : IGenericRepository<Bid, BidId>, IScopedService
{

}
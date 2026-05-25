using MazadZone.Domain.Auctions;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Sellers;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Repositories;
public class SellerRepository : GenericRepository<Seller, UserId>, ISellerRepository
{
    private readonly AppDbContext _context;
    public SellerRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Seller?> GetByAuctionIdAsync(AuctionId auctionId, CancellationToken ct)
    {
        // var query = from auction in _context.Auctions
        //             where auction.Id == auctionId
        //             join seller in _context.Sellers on auction.SellerId equals seller.Id
        //             select seller;

        // return query.FirstOrDefaultAsync(ct);
        return null;
    }

    public Task<Seller?> GetByOrderIdAsync(OrderId orderId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
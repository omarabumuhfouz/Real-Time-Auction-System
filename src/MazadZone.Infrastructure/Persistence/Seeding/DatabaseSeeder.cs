using Bogus;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Auctions.Enums;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Categories;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Sellers;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.ValueObjects;
using MazadZone.Domain.Payments.Enums;
using MzadZone.Domain.Payments;
using MzadZone.Domain.Payments.Entities;
using MazadZone.Domain.Disputes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace MazadZone.Infrastructure.Persistence.Seeding;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(AppDbContext dbContext, ILogger<DatabaseSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    private void ClearDomainEvents(object entity)
    {
        try
        {
            if (entity == null) return;
            var method = entity.GetType().GetMethod("ClearDomainEvents", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            method?.Invoke(entity, null);
        }
        catch { /* ignore safely */ }
    }

    private void SetPrivateProperty(object entity, string propertyName, object value)
    {
        var prop = entity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        prop?.SetValue(entity, value);
    }

    public async Task SeedAsync()
    {
        if (await _dbContext.Users.AnyAsync())
        {
            _logger.LogInformation("Database already seeded. Skipping...");
            return;
        }

        _logger.LogInformation("Starting database seeding with all kinds of states...");
        Randomizer.Seed = new Random(2026);
        var f = new Faker();

        // 1. Dispute Types
        var disputeTypes = new List<DisputeType>
        {
            DisputeType.Create("Item Not Received", "Buyer claims they did not receive the purchased item.").Value,
            DisputeType.Create("Item Not As Described", "Buyer claims the item condition differs from the listing description.").Value,
            DisputeType.Create("Damaged Item", "Buyer claims the item arrived damaged due to shipping or handling.").Value
        };
        foreach (var dt in disputeTypes) ClearDomainEvents(dt);
        await _dbContext.AddRangeAsync(disputeTypes);
        await _dbContext.SaveChangesAsync();

        // 2. Categories & Subcategories
        var rootCategories = new List<Category>
        {
            Category.Create("Electronics", "Phones, laptops, TVs, cameras, and other gadgets.").Value,
            Category.Create("Fashion", "Clothing, watches, shoes, and accessories.").Value,
            Category.Create("Vehicles", "Cars, motorcycles, and parts.").Value,
            Category.Create("Art & Collectibles", "Paintings, sculptures, vintage items, and memorabilia.").Value,
            Category.Create("Real Estate", "Apartments, villas, and commercial lands.").Value
        };
        foreach (var c in rootCategories) ClearDomainEvents(c);
        await _dbContext.AddRangeAsync(rootCategories);
        await _dbContext.SaveChangesAsync();

        var subCategories = new List<Category>
        {
            // Electronics
            Category.Create("Smartphones", "Mobile phones and smart devices.", rootCategories[0].Id).Value,
            Category.Create("Laptops", "Portable notebook computers.", rootCategories[0].Id).Value,
            // Fashion
            Category.Create("Watches", "Luxury wristwatches and timepieces.", rootCategories[1].Id).Value,
            Category.Create("Apparel", "Clothing and garments.", rootCategories[1].Id).Value,
            // Vehicles
            Category.Create("Sports Cars", "High-performance passenger vehicles.", rootCategories[2].Id).Value,
            Category.Create("Toys", "Replica toys and remote controlled vehicles.", rootCategories[2].Id).Value,
            // Art
            Category.Create("Paintings", "Oil and watercolor canvas artworks.", rootCategories[3].Id).Value,
            Category.Create("Sports Memorabilia", "Signed jerseys, balls, and equipment.", rootCategories[3].Id).Value
        };
        foreach (var c in subCategories) ClearDomainEvents(c);
        await _dbContext.AddRangeAsync(subCategories);
        await _dbContext.SaveChangesAsync();

        // 3. Users (Bidders, Sellers, and Mixed)
        var users = new List<User>();
        var bidders = new List<Bidder>();
        var sellers = new List<Seller>();

        for (int i = 1; i <= 15; i++)
        {
            var roles = new HashSet<UserRole> { UserRole.Bidder };
            if (i >= 9) // Users 9 to 15 will also be sellers
            {
                roles.Add(UserRole.Seller);
            }

            var user = User.Create(
                email: $"user{i}@mazadzone.com",
                passwordHash: "Hash123!",
                phoneNumber: $"07910020{i:00}",
                firstName: f.Name.FirstName(),
                secondName: f.Name.FirstName(),
                thirdName: f.Name.FirstName(),
                lastName: f.Name.LastName(),
                roles: roles
            ).Value;

            // Apply specific user states
            if (i == 14) // User 14 is Suspended
            {
                user.Suspend(Reason.Create("Violated shill bidding policies on platform.").Value, DateTime.UtcNow.AddDays(30));
            }
            else if (i == 15) // User 15 is Banned
            {
                user.Ban(Reason.Create("Abused chargeback system multiple times.").Value);
            }

            users.Add(user);

            var address = Address.Create(f.Address.City(), f.Address.StreetName(), f.Address.BuildingNumber(), "Landmark location").Value;
            var bidder = Bidder.CompleteProfile(user.Id, f.Random.Replace("##########"), address).Value;
            bidder.Verify();
            bidders.Add(bidder);

            if (roles.Contains(UserRole.Seller))
            {
                var seller = Seller.BecomeSeller(user.Id, f.Finance.Account(12), f.Random.Replace("##########")).Value;
                seller.Verify();
                sellers.Add(seller);
            }
        }

        foreach (var u in users) ClearDomainEvents(u);
        foreach (var b in bidders) ClearDomainEvents(b);
        foreach (var s in sellers) ClearDomainEvents(s);

        await _dbContext.AddRangeAsync(users);
        await _dbContext.AddRangeAsync(bidders);
        await _dbContext.AddRangeAsync(sellers);
        await _dbContext.SaveChangesAsync();

        var activeSellers = sellers.Where(s => users.First(u => u.Id == s.Id).Status == UserStatus.Active).ToList();
        var activeBidders = bidders.Where(b => users.First(u => u.Id == b.Id).Status == UserStatus.Active).ToList();

        // 4. Seeding Auctions in various states
        var categorySmartphones = subCategories[0];
        var categoryLaptops = subCategories[1];
        var categoryWatches = subCategories[2];
        var categoryApparel = subCategories[3];
        var categoryToys = subCategories[5];
        var categoryPaintings = subCategories[6];
        var categorySportsMemorabilia = subCategories[7];

        var shippingAddress = Address.Create("Amman", "Queen Rania St", "102", "Opposite to University of Jordan").Value;

        // --- Auction 1: Pending Auction (in the future) ---
        var pendingAuction = Auction.Create(
            sellerId: activeSellers[0].Id,
            status: ItemStatus.LikeNew,
            condition: Description.Create("Immaculate condition, original packaging included.").Value,
            shippingAddress: shippingAddress,
            startBidAmount: 500m,
            minBidAmount: 20m,
            startTime: DateTime.UtcNow.AddDays(2),
            endTime: DateTime.UtcNow.AddDays(7),
            title: "Vintage Leica M6 Camera",
            description: "A rare and pristine vintage camera from 1984. Perfect for collectors.",
            images: new List<Image> { Image.Create("https://picsum.photos/400", "Main View", true).Value },
            categoryId: categoryPaintings.Id
        ).Value;

        // --- Auction 2: Active Auction (No Bids) ---
        var activeNoBidsAuction = Auction.Create(
            sellerId: activeSellers[1].Id,
            status: ItemStatus.New,
            condition: Description.Create("Brand new, factory sealed.").Value,
            shippingAddress: shippingAddress,
            startBidAmount: 900m,
            minBidAmount: 15m,
            startTime: DateTime.UtcNow.AddDays(-1),
            endTime: DateTime.UtcNow.AddDays(3),
            title: "iPhone 15 Pro Max 256GB",
            description: "Sealed box titanium design. Full warranty included.",
            images: new List<Image> { Image.Create("https://picsum.photos/400", "Main View", true).Value },
            categoryId: categorySmartphones.Id
        ).Value;
        activeNoBidsAuction.MarkAsActive(DateTime.UtcNow.AddHours(-23));

        // --- Auction 3: Active Auction (With Bids) ---
        var activeWithBidsAuction = Auction.Create(
            sellerId: activeSellers[2].Id,
            status: ItemStatus.Good,
            condition: Description.Create("Pre-owned, minor desk dives on buckle.").Value,
            shippingAddress: shippingAddress,
            startBidAmount: 8000m,
            minBidAmount: 100m,
            startTime: DateTime.UtcNow.AddDays(-2),
            endTime: DateTime.UtcNow.AddDays(2),
            title: "Rolex Submariner Date",
            description: "2022 model, oyster steel, black dial, complete set.",
            images: new List<Image> { Image.Create("https://picsum.photos/400", "Main View", true).Value },
            categoryId: categoryWatches.Id
        ).Value;
        activeWithBidsAuction.MarkAsActive(DateTime.UtcNow.AddDays(-2).AddMinutes(1));

        // Simulating bids on activeWithBidsAuction
        // Bidder 1 bids 8000
        activeWithBidsAuction.PlaceBid(activeBidders[0].Id, 8000m, "auth_hold_1", DateTime.UtcNow.AddDays(-2).AddHours(2));
        // Bidder 2 bids 8100
        activeWithBidsAuction.PlaceBid(activeBidders[1].Id, 8100m, "auth_hold_2", DateTime.UtcNow.AddDays(-2).AddHours(4));
        // Bidder 3 bids 8300
        activeWithBidsAuction.PlaceBid(activeBidders[2].Id, 8300m, "auth_hold_3", DateTime.UtcNow.AddDays(-2).AddHours(6));

        // --- Auction 4: Cancelled by Seller ---
        var cancelledSellerAuction = Auction.Create(
            sellerId: activeSellers[0].Id,
            status: ItemStatus.Fair,
            condition: Description.Create("Scratches on the wooden top.").Value,
            shippingAddress: shippingAddress,
            startBidAmount: 150m,
            minBidAmount: 10m,
            startTime: DateTime.UtcNow.AddDays(1),
            endTime: DateTime.UtcNow.AddDays(4),
            title: "Drafting Table Oak",
            description: "Solid oak table with adjustable tilt mechanism.",
            images: new List<Image> { Image.Create("https://picsum.photos/400", "Main View", true).Value },
            categoryId: categoryLaptops.Id
        ).Value;
        cancelledSellerAuction.MarkAsCancelled(DateTime.UtcNow, "Seller withdrew the item due to accidental damage.");

        // --- Auction 5: Cancelled by Admin ---
        var cancelledAdminAuction = Auction.Create(
            sellerId: activeSellers[1].Id,
            status: ItemStatus.New,
            condition: Description.Create("Brand new packaging.").Value,
            shippingAddress: shippingAddress,
            startBidAmount: 300m,
            minBidAmount: 20m,
            startTime: DateTime.UtcNow.AddDays(-1),
            endTime: DateTime.UtcNow.AddDays(5),
            title: "Counterfeit Designer Bag",
            description: "High quality replica designer bag.",
            images: new List<Image> { Image.Create("https://picsum.photos/400", "Main View", true).Value },
            categoryId: categoryApparel.Id
        ).Value;
        cancelledAdminAuction.MarkAsActive(DateTime.UtcNow.AddDays(-1).AddMinutes(1));
        cancelledAdminAuction.MarkAsCancelledByAdmin();

        // --- Auction 6: Ended Auction (No Bids) ---
        var endedNoBidsAuction = Auction.Create(
            sellerId: activeSellers[2].Id,
            status: ItemStatus.Fair,
            condition: Description.Create("Slightly faded canvas colors.").Value,
            shippingAddress: shippingAddress,
            startBidAmount: 200m,
            minBidAmount: 15m,
            startTime: DateTime.UtcNow.AddDays(-5),
            endTime: DateTime.UtcNow.AddDays(-1),
            title: "Abstract Oil Painting",
            description: "A mid-century modern oil painting, artist unknown.",
            images: new List<Image> { Image.Create("https://picsum.photos/400", "Main View", true).Value },
            categoryId: categoryPaintings.Id
        ).Value;
        endedNoBidsAuction.MarkAsActive(DateTime.UtcNow.AddDays(-5).AddMinutes(1));
        endedNoBidsAuction.MarkAsEnded(DateTime.UtcNow.AddDays(-1).AddMinutes(5));

        // --- Helper list to add auctions ---
        var auctions = new List<Auction>
        {
            pendingAuction,
            activeNoBidsAuction,
            activeWithBidsAuction,
            cancelledSellerAuction,
            cancelledAdminAuction,
            endedNoBidsAuction
        };

        // --- Ended Auctions with Bids, leading to Order States ---
        var orderAuctions = new List<(Auction Auction, Bidder Winner, OrderStatus TargetStatus, bool AddFeedback, bool AddReply, DisputeStatus? TargetDisputeStatus)>();

        var orderStatuses = new[]
        {
            (TargetStatus: OrderStatus.Pending, AddFeedback: false, AddReply: false, TargetDisputeStatus: (DisputeStatus?)null),
            (TargetStatus: OrderStatus.Confirmed, AddFeedback: false, AddReply: false, TargetDisputeStatus: (DisputeStatus?)null),
            (TargetStatus: OrderStatus.Shipped, AddFeedback: false, AddReply: false, TargetDisputeStatus: (DisputeStatus?)null),
            (TargetStatus: OrderStatus.Delivered, AddFeedback: true, AddReply: false, TargetDisputeStatus: (DisputeStatus?)null),
            (TargetStatus: OrderStatus.Delivered, AddFeedback: true, AddReply: true, TargetDisputeStatus: (DisputeStatus?)null),
            (TargetStatus: OrderStatus.Delivered, AddFeedback: false, AddReply: false, TargetDisputeStatus: DisputeStatus.Open),
            (TargetStatus: OrderStatus.Delivered, AddFeedback: false, AddReply: false, TargetDisputeStatus: DisputeStatus.Resolved)
        };

        var titles = new[]
        {
            "PlayStation 5 Console",
            "MacBook Pro 16 M3 Max",
            "Tesla Model 3 Toy Replica",
            "Leather Jacket Classic",
            "Signed Football Messi",
            "Samsung 65-inch QLED TV",
            "GoPro Hero 12"
        };

        var descriptions = new[]
        {
            "Standard disc edition console with 1 controller.",
            "Apple M3 Max chip, 36GB RAM, 1TB SSD. Space black.",
            "Diecast scale replica model with working doors.",
            "Genuine lambskin leather, black color, size L.",
            "Authentic football signed by Lionel Messi. With COA.",
            "Smart TV with direct full array. HDR 10+ compatible.",
            "Waterproof action camera with accessories bundle."
        };

        var categories = new[]
        {
            categorySmartphones,
            categoryLaptops,
            categoryToys,
            categoryApparel,
            categorySportsMemorabilia,
            categorySmartphones,
            categoryPaintings
        };

        for (int i = 0; i < orderStatuses.Length; i++)
        {
            var target = orderStatuses[i];
            var auction = Auction.Create(
                sellerId: activeSellers[i % activeSellers.Count].Id,
                status: ItemStatus.LikeNew,
                condition: Description.Create("Very lightly used, perfect function.").Value,
                shippingAddress: shippingAddress,
                startBidAmount: 100m + i * 50m,
                minBidAmount: 10m,
                startTime: DateTime.UtcNow.AddDays(-10),
                endTime: DateTime.UtcNow.AddDays(-3),
                title: titles[i],
                description: descriptions[i],
                images: new List<Image> { Image.Create("https://picsum.photos/400", "Main View", true).Value },
                categoryId: categories[i].Id
            ).Value;

            auction.MarkAsActive(DateTime.UtcNow.AddDays(-10).AddMinutes(1));

            // Place bids: winner is activeBidders[(i + 1) % activeBidders.Count]
            var winner = activeBidders[(i + 1) % activeBidders.Count];
            var bidAmount = 100m + i * 50m + 20m;
            auction.PlaceBid(winner.Id, bidAmount, $"auth_intent_{i}", DateTime.UtcNow.AddDays(-5));

            auction.MarkAsEnded(DateTime.UtcNow.AddDays(-3).AddMinutes(5));

            auctions.Add(auction);
            orderAuctions.Add((auction, winner, target.TargetStatus, target.AddFeedback, target.AddReply, target.TargetDisputeStatus));
        }

        // Save all auctions and bids
        foreach (var a in auctions) ClearDomainEvents(a);
        await _dbContext.AddRangeAsync(auctions);
        await _dbContext.SaveChangesAsync();

        // 5. Create Orders, Payments, Feedbacks and Disputes
        foreach (var item in orderAuctions)
        {
            var auction = item.Auction;
            var winner = item.Winner;
            var leadingBid = auction.CurrentLeadingBid;

            var order = Order.Create(
                auction.Id,
                winner.Id,
                leadingBid.Id,
                winner.DefaultShippingAddress,
                leadingBid.Amount.Amount
            ).Value;

            // Generate Payment matching the flow
            var payment = Payment.Create(order.Id, winner.Id, leadingBid.Amount).Value;

            if (item.TargetStatus >= OrderStatus.Confirmed)
            {
                // Record Authorization Hold Success
                payment.RecordTransactionAttempt($"pi_auth_{order.Id.Value}", TransactionType.AuthorizationHold);
                payment.ResolveTransactionSuccess($"pi_auth_{order.Id.Value}");

                // Record Deposit Capture Success
                payment.RecordTransactionAttempt($"pi_cap_{order.Id.Value}", TransactionType.DepositCaptured);
                payment.ResolveTransactionSuccess($"pi_cap_{order.Id.Value}");

                // Record Remaining Amount Capture Success
                payment.RecordTransactionAttempt($"pi_rem_{order.Id.Value}", TransactionType.RemainingAmountCapture);
                payment.ResolveTransactionSuccess($"pi_rem_{order.Id.Value}");

                // Confirm Order
                order.Confirm();

                if (item.TargetStatus >= OrderStatus.Shipped)
                {
                    order.Ship();

                    if (item.TargetStatus >= OrderStatus.Delivered)
                    {
                        order.Deliver();

                        if (item.AddFeedback)
                        {
                            order.AddFeedback(5, "Absolutely beautiful product, exceeded my expectations!");

                            if (item.AddReply)
                            {
                                order.ReplyToFeedback("Thank you so much for your feedback! Enjoy it!");
                            }
                        }

                        if (item.TargetDisputeStatus != null)
                        {
                            var disputeType = disputeTypes[0]; // "Item Not Received" / "Item Not As Described"
                            var dispute = Dispute.Open(
                                order.Id,
                                disputeType.Id,
                                Title.Create("Dispute opened for verification").Value,
                                Description.Create("The item had scratches not listed in the condition.").Value,
                                new List<Image>()
                            );

                            if (item.TargetDisputeStatus == DisputeStatus.UnderReview)
                            {
                                dispute.UnderReview();
                            }
                            else if (item.TargetDisputeStatus == DisputeStatus.Resolved)
                            {
                                dispute.UnderReview();
                                dispute.Resolve(Resolution.Create("Refund of 50 JOD issued to buyer. Dispute closed.").Value);
                                SetPrivateProperty(dispute, "Status", DisputeStatus.Resolved);
                            }

                            // Associate with order
                            SetPrivateProperty(order, "DisputeId", dispute.Id);
                            ClearDomainEvents(dispute);
                            await _dbContext.AddAsync(dispute);
                        }
                    }
                }
            }
            else
            {
                // Payment Pending state: we can just add a pending transaction hold attempt
                payment.RecordTransactionAttempt($"pi_pending_{order.Id.Value}", TransactionType.AuthorizationHold);
            }

            ClearDomainEvents(order);
            ClearDomainEvents(payment);

            if (order.Feedback != null) ClearDomainEvents(order.Feedback);

            await _dbContext.AddAsync(order);
            await _dbContext.AddAsync(payment);
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("🎉 Database seeding completed successfully with all entity states!");
    }
}
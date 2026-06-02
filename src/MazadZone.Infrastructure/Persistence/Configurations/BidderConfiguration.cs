using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Infrastructure.Common.Constants;
using MazadZone.Infrastructure.Persistence.Converters; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MazadZone.Infrastructure.Persistence.Extensions;
using MazadZone.Domain.Users;

namespace MazadZone.Infrastructure.Persistence.Configurations;

internal sealed class BidderConfiguration : IEntityTypeConfiguration<Bidder>
{
    public void Configure(EntityTypeBuilder<Bidder> builder)
    {
        builder.ToTable(TableNames.Bidders);

        // Primary Key
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
               .HasConversion(new UserIdConverter()) // Ensure you have this converter
               .ValueGeneratedNever();


        // Primitives
        builder.Property(b => b.NationalId).HasMaxLength(50).IsRequired();

        builder.HasIndex(b => b.NationalId)
       .IsUnique();

        builder.Property(b => b.IsVerified).IsRequired();
        builder.Property(b => b.CreatedOnUtc).IsRequired();
        builder.Property(b => b.ModifiedOnUtc).IsRequired(false);
        builder.Property(b => b.CompletedPurchasesCount).HasDefaultValue(0);
        builder.Property(b => b.AuctionsWonCount).HasDefaultValue(0);
        builder.Property(b => b.TotalPidsPlaced).HasDefaultValue(0);
        builder.Property(b => b.AuctionParticipatedCount).HasDefaultValue(0);

        // Ignored Computed Properties
        builder.Ignore(b => b.UnpaidAuctions); // We map the backing field below instead

        builder.Property<HashSet<AuctionId>>("_unpaidAuctions")
               .HasColumnName("UnpaidAuctions")
               .HasColumnType("nvarchar(max)")
               .HasConversion(new AuctionIdSetConverter())
               .Metadata.SetValueComparer(new AuctionIdSetComparer());


        builder.ComplexProperty(b => b.DefaultShippingAddress, addressBuilder =>
           {
               addressBuilder.ConfigureAddressMapping();
           });

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Bidder>(b => b.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        
    
    }
}
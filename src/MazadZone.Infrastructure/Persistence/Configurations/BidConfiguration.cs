using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.ValueObjects;
using MazadZone.Infrastructure.Common.Constants;
using MazadZone.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MazadZone.Infrastructure.Persistence.Configurations;

class BidConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.ToTable(TableNames.Bids);
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasConversion(new BidIdConverter())
            .ValueGeneratedNever();

        builder.Property(b => b.BidderId)
            .HasConversion(new UserIdIdConverter())
            .IsRequired();

        
        builder.Property(b => b.PlacedAtUtc).IsRequired();
        builder.Property(b => b.Status).HasConversion<int>().IsRequired();
        builder.Property(b => b.GatewayAuthHoldId).IsRequired();

        builder.ComplexProperty(b => b.Amount, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("AmountCurrency")
                .HasMaxLength(AuctionConstants.MaxCurrencyCodeLength)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code)
                )
                .IsRequired();
        });

        builder.ComplexProperty(b => b.DepositAmount, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("DepositAmount")
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("DepositAmountCurrency")
                .HasMaxLength(AuctionConstants.MaxCurrencyCodeLength)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code)
                )
                .IsRequired();
        });

        builder.HasOne<Auction>()
            .WithMany()
            .HasForeignKey(b => b.AuctionId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Bidder>()
           .WithMany()
           .HasForeignKey(b => b.BidderId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Restrict);

        

    }
}
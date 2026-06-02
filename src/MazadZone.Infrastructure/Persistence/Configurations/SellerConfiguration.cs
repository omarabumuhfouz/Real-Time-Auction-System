using MazadZone.Domain.Sellers;
using MazadZone.Domain.Users;
using MazadZone.Infrastructure.Common.Constants;
using MazadZone.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MazadZone.Infrastructure.Persistence.Configurations;

internal sealed class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable(TableNames.Sellers);

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
               .HasConversion(new UserIdConverter()) 
               .ValueGeneratedNever();

        builder.Property(s => s.ListedAuctionsCount).HasDefaultValue(0);

        
        builder.Property(s => s.Rating)
               .HasColumnType("decimal(3, 2)") 
               .IsRequired();
               
        builder.Property(s => s.ReviewsCount).IsRequired();
        builder.Property(s => s.IsVerified).IsRequired();

        builder.Property(s => s.CreatedOnUtc).IsRequired();
        builder.Property(s => s.ModifiedOnUtc).IsRequired(false);

        builder.HasOne<User>()
        .WithOne()
        .HasForeignKey<Seller>(s => s.Id)
        .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);
    
    }
}
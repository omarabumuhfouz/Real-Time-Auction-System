using MazadZone.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Data.Config;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<HashedRefreshToken>
{
    public void Configure(EntityTypeBuilder<HashedRefreshToken> builder)
    {
        // Table name (optional)
        builder.ToTable("RefreshTokens");

        // Primary Key
        builder.HasKey(rt => rt.Id);

        // Unique Index
        builder.HasIndex(rt => rt.Token)
               .IsUnique()
               .HasDatabaseName("IX_Token_Unique");

        // Properties
        builder.Property(rt => rt.Token)
               .IsRequired();

        builder.Property(rt => rt.UserId)
               .IsRequired();


        builder.Property(rt => rt.ExpiresAt)
               .IsRequired();

        builder.Property(rt => rt.IsRevoked)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(rt => rt.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()"); // for SQL Server

        builder.Property(rt => rt.RevokedAt)
               .IsRequired(false);

    }
}

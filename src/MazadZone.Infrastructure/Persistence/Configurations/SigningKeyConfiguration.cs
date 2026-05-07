using MazadZone.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Data.Config;

public class SigningKeyConfiguration : IEntityTypeConfiguration<SigningKey>
{
    public void Configure(EntityTypeBuilder<SigningKey> builder)
    {
        builder.ToTable("SigningKeys");

        builder.HasKey(sk => sk.Id);

        builder.Property(sk => sk.KeyId)
               .IsRequired()
               .HasMaxLength(SigningKeySettings.KeyMaxLength);

        builder.Property(sk => sk.PrivateKey)
               .IsRequired();

        builder.Property(sk => sk.PublicKey)
               .IsRequired();

        builder.Property(sk => sk.IsActive)
               .IsRequired();

        builder.Property(sk => sk.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()"); // optional default

        builder.Property(sk => sk.ExpiresAt)
               .IsRequired();
    }
}

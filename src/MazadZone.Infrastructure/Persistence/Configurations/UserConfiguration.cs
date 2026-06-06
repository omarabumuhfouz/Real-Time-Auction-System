using AuthService.Domain.Constants;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Users;
using MazadZone.Infrastructure.Common.Constants;
using MazadZone.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MazadZone.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // 1. Table Mapping
        builder.ToTable(TableNames.Users);
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasConversion(new UserIdConverter())
            .ValueGeneratedNever();

        // 2. Multi-Property Value Objects (Complex Types)
        builder.ComplexProperty(u => u.FullName, nameBuilder =>
        {
            nameBuilder.Property(f => f.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(UserConstants.NameMaxLength)
                .IsRequired();

            nameBuilder.Property(f => f.SecondName)
                .HasColumnName("SecondName")
                .HasMaxLength(UserConstants.NameMaxLength)
                .IsRequired();

            nameBuilder.Property(f => f.ThirdName)
                .HasColumnName("ThirdName")
                .HasMaxLength(UserConstants.NameMaxLength)
                .IsRequired();

            nameBuilder.Property(f => f.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(UserConstants.NameMaxLength)
                .IsRequired();
        });

        builder.Property(u => u.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasMaxLength(UserConstants.PhoneNumberLength)
            .IsRequired(true)
            .HasConversion(new PhoneNumberConverter());


        builder.Property(u => u.Email)
            .HasConversion(new EmailConverter())
            .HasMaxLength(UserConstants.EmailMaxLength)
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(u => u.LastLogin)
        .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasConversion(new PasswordHashConverter())
            .HasMaxLength(UserConstants.PasswordHashLength)
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(u => u.EnforcementReason)
        .HasConversion(new ReasonConverter());

        builder.Property(u => u.Status)
            .HasConversion<int>()
            .HasColumnType("int")
            .IsRequired();

        builder.Property(u => u.Roles)
    .HasConversion<int>() // Automatically handles the bitwise operations natively
    .HasColumnName("Roles")
    .HasColumnType("int")
    .IsRequired();



        builder.HasMany(u => u.HashedRefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.HashedRefreshTokens)
            .HasField("_hashedRefreshTokens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(u => u.PaymentMethods)
            .WithOne()
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.PaymentMethods)
            .HasField("_paymentMethods")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
    }
}
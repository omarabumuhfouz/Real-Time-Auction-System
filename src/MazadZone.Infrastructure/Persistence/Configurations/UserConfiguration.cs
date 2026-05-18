using System;
using System.Collections.Generic;
using System.Linq;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MazadZone.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // 1. Table Mapping
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasConversion(new UserIdIdConverter())
            .ValueGeneratedNever();

        // 2. Multi-Property Value Objects (Complex Types)
        builder.ComplexProperty(u => u.FullName, nameBuilder =>
        {
            nameBuilder.Property(f => f.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(50)
                .IsRequired();

            nameBuilder.Property(f => f.SecondName)
                .HasColumnName("SecondName")
                .HasMaxLength(50)
                .IsRequired();

            nameBuilder.Property(f => f.ThirdName)
                .HasColumnName("ThirdName")
                .HasMaxLength(50)
                .IsRequired();

            nameBuilder.Property(f => f.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(50)
                .IsRequired();
        });

builder.ComplexProperty(u => u.PhoneNumber, phoneBuilder =>
{
    phoneBuilder.Property(p => p.Value)
        .HasColumnName("PhoneNumber")
        .HasMaxLength(9);

    // This is where you define the complex property itself as optional
    phoneBuilder.IsRequired(false); 
});

        // 3. Single-Property Value Objects (Value Converters)

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value).Value
            )
            .HasMaxLength(255)
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasConversion(
                passwordHash => passwordHash.Value,
                value => PasswordHash.Create(value).Value
            )
            .HasMaxLength(255)
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // 4. Enums
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Map Roles HashSet as a comma-separated string for readability
        builder.Property(u => u.Roles)
            .HasConversion(
                roles => string.Join(',', roles.Select(r => r.ToString())),
                value => string.IsNullOrWhiteSpace(value) 
                    ? new HashSet<UserRole>() 
                    : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(Enum.Parse<UserRole>)
                           .ToHashSet()
            )
            .HasColumnName("Roles")
            .HasMaxLength(500)
            // 5. Encapsulation: Explicitly instructs EF to read/write the backing field for private setters
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
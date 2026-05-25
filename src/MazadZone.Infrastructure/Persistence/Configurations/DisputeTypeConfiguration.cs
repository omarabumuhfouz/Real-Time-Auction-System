using MazadZone.Domain.Disputes;
using MazadZone.Domain.Orders; // Adjust if you placed it in MazadZone.Domain.Support
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Infrastructure.Common.Constants;
using MazadZone.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MazadZone.Infrastructure.Persistence.Configurations;

public class DisputeTypeConfiguration : IEntityTypeConfiguration<DisputeType>
{
    public void Configure(EntityTypeBuilder<DisputeType> builder)
    {
        // 1. Table Name
        builder.ToTable(TableNames.DisputeTypes);

        // 2. Primary Key & Strongly-Typed ID Conversion
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
               .HasConversion(new DisputeTypeIdConverter())
               .IsRequired();

        builder.Property(t => t.Name)
               .HasConversion(new NameConverter())
               .HasMaxLength(100) 
               .IsRequired();

        builder.Property(t => t.Description)
               .HasConversion(new DescriptionConverter())
               .HasMaxLength(500) 
               .IsRequired();

        // 4. Primitive Properties
        builder.Property(t => t.IsActive)
               .IsRequired();

        builder.Property(t => t.IsDeleted)
               .IsRequired();

        builder.Property(t => t.DeletedOnUtc)
               .IsRequired(false);

        builder.Property(t => t.CreatedOnUtc)
               .IsRequired();

        builder.Property(t => t.ModifiedOnUtc)
               .IsRequired(false);

              builder.HasMany<Dispute>()
              .WithOne()
              .HasForeignKey(d => d.DisputeTypeId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Cascade);

              

        // 5. Global Query Filter for Soft Delete
        // This ensures queries like _context.DisputeTypes.ToList() will automatically exclude deleted items.
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
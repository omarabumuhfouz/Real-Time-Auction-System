namespace MazadZone.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MazadZone.Domain.Orders;
using MazadZone.Infrastructure.Persistence.Converters;
using MazadZone.Infrastructure.Common.Constants;
using MazadZone.Domain.Shared;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Disputes;

public sealed class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> builder)
    {
        builder.ToTable(TableNames.Disputes);

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(new DisputeIdConverter());

        builder.Property(d => d.DisputeTypeId)
            .IsRequired()
            .HasConversion(new DisputeTypeIdConverter());


        builder.Property(d => d.OrderId)
            .IsRequired()
            .HasConversion(new OrderIdConverter());

        builder.Property(d => d.Description)
            .IsRequired()
            .HasColumnName("Description")
            .HasMaxLength(SharedConstainst.MaxDescriptionLength)
            .HasConversion(new DescriptionConverter());

        builder.Property(d => d.Title)
            .IsRequired()
            .HasColumnName("Title")
            .HasMaxLength(SharedConstainst.MaxTitleLength)
            .HasConversion(new TitleConverter());

        builder.Property(d => d.Resolution)
            .IsRequired()
            .HasColumnName("Resolution")
            .HasMaxLength(OrderConstants.MaxResolutionLength)
            .HasConversion(new ResolutionConverter());

        builder.Property(d => d.Status)
            .IsRequired(); 

        builder.Property(d => d.CreatedAtUtc)
            .IsRequired();

        builder.Property(d => d.ResolvedAtUtc)
            .IsRequired(false);

        builder.OwnsMany(d => d.Images, imageBuilder =>
                {
                    // 1. Tell EF Core to store these in a separate table called "DisputeImages"
                    imageBuilder.ToTable("DisputeImages");

                    imageBuilder.Property(i => i.Path)
                    .HasColumnName("ImageUrl");

                    imageBuilder.Property(i => i.AltText);

                    // 2. Link them back to the parent Dispute
                    imageBuilder.WithOwner().HasForeignKey("DisputeId");

                    // 3. Configure the internal primary key for this new table
                    // If your Image object has an Id property, use that. 
                    // Otherwise, let EF Core create a hidden "shadow" key:
                    imageBuilder.HasKey("Id");


                    // 4. Configure the actual properties inside your Image class (e.g., Url)
                    // imageBuilder.Property(i => i.Url).IsRequired().HasMaxLength(2048);
                });

        builder.HasOne<DisputeType>()
        .WithMany()
        .HasForeignKey(d => d.DisputeTypeId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(d => d.IsResolved);
    }
}
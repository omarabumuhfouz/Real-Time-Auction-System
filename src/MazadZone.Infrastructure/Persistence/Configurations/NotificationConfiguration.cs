using MazadZone.Domain.Notifications;
using MazadZone.Domain.Users;
using MazadZone.Infrastructure.Common.Constants;
using MazadZone.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MazadZone.Infrastructure.Persistence.Configurations;

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable(TableNames.Notifications);

        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.Id)
            .HasConversion(new NotificationIdConverter())
            .ValueGeneratedNever(); 

        builder.Property(n => n.UserId)
            .HasConversion(new UserIdConverter())
            .IsRequired();

        // Using the constant for Title
        builder.Property(n => n.Title)
            .HasMaxLength(NotificationConstraints.TitleMaxLength)
            .IsRequired();

        // Using the constant for Message
        builder.Property(n => n.Message)
            .HasMaxLength(NotificationConstraints.MessageMaxLength)
            .IsRequired();

        builder.Property(n => n.IsRead)
            .HasDefaultValue(false);

        builder.Property(n => n.CreatedOnUtc)
            .IsRequired();

        builder.Property(n => n.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(n => !n.IsDeleted);

        builder.HasIndex(n => new { n.UserId, n.IsRead });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
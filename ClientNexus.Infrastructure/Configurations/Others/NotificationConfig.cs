using ClientNexus.Domain.Entities.Others;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Others
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder
                .Property(n => n.Id)
                .HasColumnType("BINARY(16)")
                .HasConversion(n => n.ToByteArray(), n => new Ulid(n));
            builder.Property(n => n.Title).HasColumnType("nvarchar(300)").IsRequired();
            builder.Property(n => n.Body).HasColumnType("nvarchar(2000)").IsRequired();

            builder
                .HasOne(n => n.BaseUser)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.BaseUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

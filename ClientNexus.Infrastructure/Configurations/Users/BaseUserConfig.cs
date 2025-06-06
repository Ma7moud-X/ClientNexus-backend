using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Users;

public class BaseUserConfig : IEntityTypeConfiguration<BaseUser>
{
    public void Configure(EntityTypeBuilder<BaseUser> builder)
    {
        builder.ToTable("BaseUsers");

        builder.Property(u => u.FirstName).HasColumnType("nvarchar(50)").IsRequired();
        builder.Property(u => u.LastName).HasColumnType("nvarchar(50)").IsRequired();
        builder.Property(u => u.PhoneNumber).HasColumnType("varchar(20)");
        builder
            .Property(u => u.UserType)
            .HasConversion(ut => (char)ut, ut => (UserType)ut)
            .HasColumnType("char(1)")
            .IsRequired();

        builder.Property(u => u.NotificationToken).HasColumnType("varchar(1000)");

        builder
            .HasOne(b => b.BlockedBy)
            .WithMany(b => b.BlockedUsers)
            .HasForeignKey(b => b.BlockedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(b => b.PhoneNumbers)
            .WithOne(b => b.BaseUser)
            .HasForeignKey(b => b.BaseUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(sp => sp.MainImage).HasMaxLength(1000);

    }
}

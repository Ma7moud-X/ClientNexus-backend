using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users;

public class BaseUserConfig : IEntityTypeConfiguration<BaseUser>
{
    public void Configure(EntityTypeBuilder<BaseUser> builder)
    {
        builder.ToTable("BaseUsers");

        builder.Property(u => u.FirstName).HasColumnType("nvarchar(50)").IsRequired();
        builder.Property(u => u.LastName).HasColumnType("nvarchar(50)").IsRequired();
        builder.Property(u => u.PhoneNumber).HasColumnType("varchar(20)");

        builder.HasOne(u => u.BlockedBy)
            .WithMany(a => a.BlockedUsers)
            .HasForeignKey(u => u.BlockedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.PhoneNumbers)
            .WithOne(a => a.BaseUser)
            .HasForeignKey(u => u.BaseUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

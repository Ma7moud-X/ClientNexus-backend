using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users;

public class AdminConfig : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins");

        builder.Property(a => a.ApprovedById).IsRequired(false);
        
        
        builder
            .HasOne(a => a.ApprovingAdmin)
            .WithMany(a => a.ApprovedAdmins)
            .HasForeignKey(a => a.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.AccessLevel)
            .WithMany(a => a.Admins)
            .HasForeignKey(a => a.AccessLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(a => a.Problems)
            .WithOne(a => a.Admin)
            .HasForeignKey(a => a.AdminId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(a => a.Documents)
            .WithOne(a => a.UploadedBy)
            .HasForeignKey(a => a.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(a => a.BlockedUsers)
            .WithOne(a => a.BlockedBy)
            .HasForeignKey(a => a.BlockedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

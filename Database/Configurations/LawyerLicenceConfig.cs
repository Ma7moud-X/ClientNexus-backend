using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class LawyerLicenceConfig : IEntityTypeConfiguration<LawyerLicence>
    {
        public void Configure(EntityTypeBuilder<LawyerLicence> builder)
        {
            builder.ToTable("LawyerLicences");

            builder.Property(l => l.LicenceNumber)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(l => l.IssuingAuthority)
                .IsRequired(true)
                .HasMaxLength(100);

            builder.Property(l => l.IssueDate)
                .IsRequired(true);

            builder.Property(l => l.ExpiryDate)
                .IsRequired(true);

            builder.HasOne(l => l.Lawyer)
                .WithMany(l => l.Licences)
                .HasForeignKey(l => l.LawyerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
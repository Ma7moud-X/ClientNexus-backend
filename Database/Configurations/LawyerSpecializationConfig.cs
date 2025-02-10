using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class LawyerSpecializationConfig : IEntityTypeConfiguration<LawyerSpecialization>
    {
        public void Configure(EntityTypeBuilder<LawyerSpecialization> builder)
        {
            builder.ToTable("LawyerSpecializations");

            builder.Property(s => s.Name)
                .IsRequired(true)
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.HasOne(s => s.Lawyer)
                .WithMany(l => l.Specializations)
                .HasForeignKey(s => s.LawyerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
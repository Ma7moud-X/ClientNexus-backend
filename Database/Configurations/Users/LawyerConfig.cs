using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users
{
    public class LawyerConfig : IEntityTypeConfiguration<Lawyer>
    {
        public void Configure(EntityTypeBuilder<Lawyer> builder)
        {
            builder.ToTable("Lawyers");

            builder.Property(l => l.YearsOfExperience)
                .IsRequired(false);

            // Configure Licences relationship
            builder.HasMany(l => l.Licences)
                .WithOne(l => l.Lawyer)
                .HasForeignKey(l => l.LawyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Specializations relationship
            builder.HasMany(l => l.Specializations)
                .WithOne(l => l.Lawyer)
                .HasForeignKey(l => l.LawyerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
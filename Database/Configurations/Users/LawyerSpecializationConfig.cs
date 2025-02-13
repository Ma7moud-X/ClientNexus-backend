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

            builder.HasKey( d => new {d.Id, d.LawyerId});

            builder.Property(ls => ls.Name)
                .IsRequired(true)
                .HasMaxLength(100);

            builder
                .HasOne(ls => ls.Lawyer)
                .WithMany(ls => ls.Specializations)
                .HasForeignKey(ls => ls.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
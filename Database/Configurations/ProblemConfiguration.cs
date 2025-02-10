using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
    {
        public void Configure(EntityTypeBuilder<Problem> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Description)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.ReportedBy)
                .IsRequired()
                .HasConversion<string>();

            // Client relationship
            builder.HasOne(p => p.Client)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceProvider relationship
            builder.HasOne(p => p.ServiceProvider)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin relationship
            builder.HasOne(p => p.Admin)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.AdminId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

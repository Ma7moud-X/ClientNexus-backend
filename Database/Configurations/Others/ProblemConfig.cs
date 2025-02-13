using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class ProblemConfig : IEntityTypeConfiguration<Problem>
    {
        public void Configure(EntityTypeBuilder<Problem> builder)
        {
            
            builder.ToTable("Problems");

            builder.HasKey( d => new {d.ClientId, d.ServiceProviderId, d.AdminId});

            builder.Property(p => p.Description)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.ReportedBy)
                .IsRequired()
                .HasConversion<string>();

            builder.HasOne(p => p.Client)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ServiceProvider)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Admin)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

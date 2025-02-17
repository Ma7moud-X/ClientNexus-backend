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

            builder.Property(p => p.Description).HasColumnType("nvarchar(1000)").IsRequired();

            builder
                .Property(p => p.Status)
                .IsRequired()
                .HasColumnType("varchar(1)")
                .HasDefaultValue((char)ProblemStatus.New);

            builder.Property(p => p.ReportedBy).HasColumnType("varchar(1)").IsRequired();

            builder
                .HasOne(p => p.Client)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(p => p.ServiceProvider)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(p => p.Admin)
                .WithMany(p => p.Problems)
                .HasForeignKey(p => p.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(p => p.Service)
                .WithOne(s => s.Problem)
                .HasForeignKey<Problem>(p => p.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

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

            builder.HasKey(p => new { p.ServiceProviderId, p.Id });

            builder.Property(p => p.Id).UseIdentityColumn();

            builder.Property(p => p.Description).HasColumnType("nvarchar(1000)").IsRequired();

            builder
                .Property(p => p.Status)
                .IsRequired()
                .HasColumnType("varchar(1)")
                .HasConversion(s => (char)s, s => (ProblemStatus)s);

            builder
                .Property(p => p.ReportedBy)
                .HasConversion(rt => (char)rt, rt => (ReporterType)rt)
                .HasColumnType("varchar(1)")
                .IsRequired();

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
                .HasOne(p => p.SolvingAdmin)
                .WithMany(p => p.AssignedProblems)
                .HasForeignKey(p => p.SolvingAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(p => p.Service)
                .WithOne(s => s.Problem)
                .HasForeignKey<Problem>(p => p.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using ClientNexus.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations
{
    public class CaseFileConfiguration : IEntityTypeConfiguration<CaseFile>
    {
        public void Configure(EntityTypeBuilder<CaseFile> builder)
        {
            builder.ToTable("CaseFiles");

            builder.HasKey(c => c.Id);

            builder.Property(cf => cf.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder
                .HasOne(cf => cf.ConsultCase)
                .WithMany(cf => cf.CaseFiles)
                .HasForeignKey(cf => cf.ConsultCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class CaseFileConfiguration : IEntityTypeConfiguration<CaseFile>
    {
        public void Configure(EntityTypeBuilder<CaseFile> builder)
        {
            builder.ToTable("CaseFiles");

            builder.HasKey( d => new {d.Id, d.ConsultCaseId});

            builder.Property(cf => cf.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(cf => cf.ConsultCase)
                .WithMany(cc => cc.CaseFiles)
                .HasForeignKey(cf => cf.ConsultCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
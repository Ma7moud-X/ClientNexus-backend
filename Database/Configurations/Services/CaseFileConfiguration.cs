using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class CaseFileConfiguration : IEntityTypeConfiguration<CaseFile>
    {
        public void Configure(EntityTypeBuilder<CaseFile> builder)
        {
            // Configure table
            builder.ToTable("CaseFiles");

            // Configure primary key
            builder.HasKey(cf => cf.Id);

            // Configure properties
            builder.Property(cf => cf.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Configure relationship with ConsultationCase
            builder.HasOne(cf => cf.ConsultCase)
                .WithMany(cc => cc.CaseFiles)
                .HasForeignKey(cf => cf.ConsultCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
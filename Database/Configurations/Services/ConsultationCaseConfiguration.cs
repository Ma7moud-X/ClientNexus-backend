using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class ConsultationCaseConfiguration : IEntityTypeConfiguration<ConsultationCase>
    {
        public void Configure(EntityTypeBuilder<ConsultationCase> builder)
        {

            builder.ToTable("ConsultationCases");

            builder.HasBaseType<Service>();

            builder
                .HasMany(cf => cf.CaseFiles)
                .WithOne(cf => cf.ConsultCase)
                .HasForeignKey(cf => cf.ConsultCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
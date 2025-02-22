using ClientNexus.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations
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
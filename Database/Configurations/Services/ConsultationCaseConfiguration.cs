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

            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(cc => cc.ServiceProvider)
                .WithMany(sp => sp.ConsultationCases)
                .HasForeignKey(cc => cc.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(cc => cc.CaseFiles)
                .WithOne(cf => cf.ConsultCase)
                .HasForeignKey(cf => cf.ConsultCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class QuestionConfig : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {

            builder.ToTable("Questions");

            // Configure inheritance
            builder.HasBaseType<Service>();

            // Configure properties
            builder.Property(q => q.Visibility)
                .IsRequired()
                .HasDefaultValue(false);

            // Configure relationships
            builder.HasOne(q => q.ServiceProvider)
                .WithMany(sp => sp.Questions)
                .HasForeignKey(q => q.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
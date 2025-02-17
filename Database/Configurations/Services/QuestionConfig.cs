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

            builder.HasBaseType<Service>();

            builder.Property(q => q.Visibility)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
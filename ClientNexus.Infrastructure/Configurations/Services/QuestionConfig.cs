using ClientNexus.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations
{
    public class QuestionConfig : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {

            builder.ToTable("Questions");

            builder.HasBaseType<Service>();

            builder.Property(q => q.QuestionBody).IsRequired().HasMaxLength(2000);

            builder.Property(q => q.Visibility).HasDefaultValue(true);

            builder.Property(q => q.AnswerBody).HasMaxLength(2000);

        }
    }
}
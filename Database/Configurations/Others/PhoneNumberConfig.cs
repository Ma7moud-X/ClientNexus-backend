using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

public class PhoneNumberConfig : IEntityTypeConfiguration<PhoneNumber>
{
    public void Configure(EntityTypeBuilder<PhoneNumber> builder)
    {
        builder.ToTable("PhoneNumbers");

        builder.HasKey(p => new { p.Id, p.BaseUserId });

        builder.Property(n => n.Number).HasColumnType("varchar(20)").IsRequired();

        builder
            .HasOne(u => u.BaseUser)
            .WithMany(p => p.PhoneNumbers)
            .HasForeignKey(u => u.BaseUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

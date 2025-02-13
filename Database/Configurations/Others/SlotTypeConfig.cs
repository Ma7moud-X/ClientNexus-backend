using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Services
{
    public class SlotTypeConfig : IEntityTypeConfiguration<SlotType>
    {
        public void Configure(EntityTypeBuilder<SlotType> builder)
        {
            builder.ToTable("SlotTypes");

            builder.HasKey( s => new {s.Id, s.SlotId});

            builder.Property(s => s.Type)
                .IsRequired();

            builder
                .HasOne(s => s.Slot)
                .WithMany(s => s.SlotTypes)
                .HasForeignKey(s => s.SlotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
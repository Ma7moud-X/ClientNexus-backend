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

            builder.HasKey(st => st.Id);

            builder.Property(st => st.Type)
                .IsRequired();

            // Configure relationship with Slot
            builder.HasOne(st => st.Slot)
                .WithMany(s => s.SlotTypes)
                .HasForeignKey(s => s.SlotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
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

            builder.HasKey( d => new {d.Id, d.SlotId});

            builder.Property(st => st.Type)
                .IsRequired();

            builder.HasOne(st => st.Slot)
                .WithMany(s => s.SlotTypes)
                .HasForeignKey(s => s.SlotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
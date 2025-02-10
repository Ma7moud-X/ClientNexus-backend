using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            // Configure inherited properties from BaseUser
            builder.HasBaseType<BaseUser>();

            // Configure Rate property
            builder.Property(c => c.Rate)
                .IsRequired()
                .HasDefaultValue(0.0f);

            // Configure Problems relationship
            builder.HasMany(c => c.Problems)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users
{
    public class ClientConfig : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {

            builder.ToTable("Clients");

            builder.HasBaseType<BaseUser>();

            builder.Property(c => c.Rate)
                .IsRequired()
                .HasDefaultValue(0.0f);

            builder.HasMany(c => c.Problems)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(c => c.Payments)
               .WithOne(p => p.Client)
               .HasForeignKey(p => p.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Services)
                .WithOne(s => s.Client)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(sp => sp.ClientServiceProviderFeedbacks)
                .WithOne(sp => sp.Client)
                .HasForeignKey(sp => sp.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

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

            builder
                .HasMany(cc => cc.Problems)
                .WithOne(cc => cc.Client)
                .HasForeignKey(cc => cc.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder
                .HasMany(cc => cc.Payments)
               .WithOne(cc => cc.Client)
               .HasForeignKey(cc => cc.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(cc => cc.Services)
                .WithOne(cc => cc.Client)
                .HasForeignKey(cc => cc.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // builder
            //     .HasMany(cc => cc.ClientServiceProviderFeedbacks)
            //     .WithOne(cc => cc.Client)
            //     .HasForeignKey(cc => cc.ClientId)
            //     .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

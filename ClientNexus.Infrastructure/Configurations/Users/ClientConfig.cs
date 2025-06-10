using ClientNexus.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Users
{
    public class ClientConfig : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {

            builder.ToTable("Clients");

            builder.HasBaseType<BaseUser>();

            //builder.Property(c => c.Rate)
            //    .IsRequired()
            //    .HasDefaultValue(0.0f);

            builder
                .HasMany(cc => cc.Problems)
                .WithOne(cc => cc.Client)
                .HasForeignKey(cc => cc.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

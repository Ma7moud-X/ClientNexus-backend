using System;
using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users;

public class BaseUserConfig : IEntityTypeConfiguration<BaseUser>
{
    public void Configure(EntityTypeBuilder<BaseUser> builder)
    {
        builder.ToTable("BaseUsers");

        // data types
        builder.Property(u => u.FirstName).HasColumnType("nvarchar(50)").IsRequired();
        builder.Property(u => u.LastName).HasColumnType("nvarchar(50)").IsRequired();
        builder.Property(u => u.PhoneNumber).HasColumnType("varchar(20)");
    }
}

using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration
: IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("CUS_NAME");

        builder.HasKey(e => e.CusId);

        builder.Property(e => e.CusId)
            .HasMaxLength(5)
            .IsUnicode(false)
            .HasDefaultValue("")
            .HasColumnName("Cus_id");
        builder.Property(e => e.CusName)
            .HasMaxLength(30)
            .HasDefaultValue("")
            .HasColumnName("Cus_name");
    }
}

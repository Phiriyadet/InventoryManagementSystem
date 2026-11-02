using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class OrderHeaderConfiguration
: IEntityTypeConfiguration<OrderHeader>
{
    public void Configure(EntityTypeBuilder<OrderHeader> builder)
    {
        builder.ToTable("H_ORDER");

        builder.HasKey(e => e.OrderNo);

        builder.Property(e => e.OrderNo)
            .HasColumnName("Order_no");
        builder.Property(e => e.CusId)
            .HasMaxLength(5)
            .IsUnicode(false)
            .HasDefaultValue("")
            .HasColumnName("Cus_id");
        builder.Property(e => e.OrderDate)
            .HasDefaultValue(new
            DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
            .HasColumnType("datetime")
            .HasColumnName("Order_Date");

        builder.HasOne(d => d.Customer).WithMany(p => p.Orders)
            .HasForeignKey(d => d.CusId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_H_ORDER_CUS");
    }
}

using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class OrderDetailConfiguration
: IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("D_ORDER");

        builder.HasKey(e => new
        {
            e.OrderNo,
            e.GoodsId
        });

        builder.Property(e => e.OrderNo)
            .HasColumnName("Order_no");
        builder.Property(e => e.GoodsId)
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValue("")
            .HasColumnName("Goods_id");
        builder.Property(e => e.Amount)
            .HasColumnType("numeric(10, 2)");
        builder.Property(e => e.CostUnit)
            .HasColumnType("numeric(8, 2)")
            .HasColumnName("COST_UNIT");
        builder.Property(e => e.FinDate)
            .HasColumnType("datetime")
            .HasColumnName("Fin_date");
        builder.Property(e => e.OrdDate)
            .HasDefaultValue(new
            DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
            .HasColumnType("datetime")
            .HasColumnName("Ord_date");
        builder.Property(e => e.TotPrc)
            .HasColumnType("numeric(10, 2)")
            .HasColumnName("TOT_PRC");

        builder.HasOne(d => d.Goods).WithMany(p => p.OrderDetails)
            .HasForeignKey(d => d.GoodsId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_D_ORDER_G");

        builder.HasOne(d => d.Header).WithMany(p => p.Details)
            .HasForeignKey(d => d.OrderNo)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_D_ORDER_H");
    }
}

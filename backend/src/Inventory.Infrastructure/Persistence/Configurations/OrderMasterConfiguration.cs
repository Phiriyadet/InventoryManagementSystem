using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class OrderMasterConfiguration
: IEntityTypeConfiguration<OrderMaster>
{
    public void Configure(EntityTypeBuilder<OrderMaster> builder)
    {
        builder.ToTable("M_ORDER");

        builder.HasKey(e => new
        {
            e.CusId,
            e.GoodsId,
            e.DocDate,
            e.OrdDate,
            e.FinDate
        });

        builder.Property(e => e.CusId)
            .HasMaxLength(5)
            .IsUnicode(false)
            .HasDefaultValue("")
            .HasColumnName("Cus_id");
        builder.Property(e => e.GoodsId)
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValue("")
            .HasColumnName("Goods_id");
        builder.Property(e => e.DocDate)
            .HasDefaultValue(new
            DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
            .HasColumnType("datetime")
            .HasColumnName("Doc_date");
        builder.Property(e => e.OrdDate)
            .HasDefaultValue(new
            DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
            .HasColumnType("datetime")
            .HasColumnName("Ord_date");
        builder.Property(e => e.FinDate)
            .HasDefaultValue(new
            DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
            .HasColumnType("datetime")
            .HasColumnName("Fin_date");
        builder.Property(e => e.Amount)
            .HasColumnType("numeric(10, 2)")
            .HasColumnName("Amount");
        builder.Property(e => e.CostTot)
            .HasColumnType("numeric(10, 2)")
            .HasColumnName("cost_tot");
        builder.Property(e => e.SysDate)
            .HasDefaultValue(new
            DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
            .HasColumnType("datetime")
            .HasColumnName("Sys_date");
    }
}

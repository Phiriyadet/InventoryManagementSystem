using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class GoodsConfiguration
: IEntityTypeConfiguration<Goods>
{
    public void Configure(EntityTypeBuilder<Goods> builder)
    {
        builder.ToTable("GOODS_NAME");

        builder.HasKey(e => e.GoodsId);

        builder.Property(e => e.GoodsId)
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValue("")
            .HasColumnName("Goods_id");
        builder.Property(e => e.CostUnit)
            .HasColumnType("numeric(8, 2)")
            .HasColumnName("cost_unit");
        builder.Property(e => e.GoodsName)
            .HasMaxLength(30)
            .HasDefaultValue("")
            .HasColumnName("Goods_name");
    }
}

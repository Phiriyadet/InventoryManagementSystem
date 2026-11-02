using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Domain.Entities;

public class Goods // table GOODS_NAME
{
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public decimal CostUnit { get; set; }

    public List<OrderDetail> OrderDetails { get; set; } = new();
}

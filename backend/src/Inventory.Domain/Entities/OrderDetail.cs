namespace Inventory.Domain.Entities;

public class OrderDetail
{
    public int OrderNo { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public DateTime OrdDate { get; set; }
    public DateTime FinDate { get; set; }
    public decimal Amount { get; set; }
    public decimal CostUnit { get; set; }
    public decimal TotPrc { get; set; }

    public OrderHeader? Header { get; set; } 
    public Goods? Goods { get; set; }
}

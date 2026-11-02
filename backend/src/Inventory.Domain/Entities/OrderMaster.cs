namespace Inventory.Domain.Entities;

public class OrderMaster
{
    public string CusId { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public DateTime DocDate { get; set; }
    public DateTime OrdDate { get; set; }
    public DateTime FinDate { get; set; }
    public DateTime SysDate { get; set; }
    public decimal Amount { get; set; }
    public decimal CostTot { get; set; }
}

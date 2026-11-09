namespace Inventory.Domain.Entities;

public class OrderHeader
{
    public int OrderNo { get; set; }
    public string CusId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }

    public Customer Customer { get; set; } = null!;
    public List<OrderDetail> Details { get; set; } = new();
}

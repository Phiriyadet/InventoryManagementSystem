namespace Inventory.Domain.Entities;

public class Customer
{
    public string CusId { get; set; } = string.Empty;
    public string CusName { get; set; } = string.Empty;
 
    public List<OrderHeader> Orders { get; set; } = new();
}

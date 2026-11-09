namespace Inventory.Application.Orders.DTOs;

public record CreateHOrderRequest(
    string CusId,
    DateTime OrderDate
);

namespace Inventory.Application.Orders.DTOs;

public record UpdateDOrderRequest
(
    string GoodsId,
    DateTime OrdDate,
    DateTime? FinDate,
    decimal Amount
);

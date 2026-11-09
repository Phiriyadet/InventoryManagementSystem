namespace Inventory.Application.Orders.DTOs;

public record CreateDOrderRequest
(
    int OrderNo,
    string GoodsId,
    DateTime OrdDate,
    DateTime? FinDate,
    decimal Amount
);

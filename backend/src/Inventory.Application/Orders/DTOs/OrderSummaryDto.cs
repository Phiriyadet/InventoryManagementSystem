namespace Inventory.Application.Orders.DTOs;

public record OrderSummaryDto(
    string CusId,
    string CusName,
    int OrderNo,
    int Count,
    decimal Amount
);

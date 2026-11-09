namespace Inventory.Application.Reports.DTOs;

public record OverdueOrderReportItemDto(
    int Sequence,
    string CusId,
    string CusName,
    string GoodsId,
    string GoodsName,
    DateTime DocDate,
    int OrderNo,
    DateTime OrdDate,
    decimal Amount,
    decimal UnitCost,
    decimal TotalPrice
);

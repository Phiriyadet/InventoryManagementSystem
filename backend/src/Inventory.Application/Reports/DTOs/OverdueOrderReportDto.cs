namespace Inventory.Application.Reports.DTOs;

public record OverdueOrderReportDto(
    IReadOnlyList<OverdueOrderReportItemDto> Items,
    decimal TotalAmount,
    decimal TotalPrice
);
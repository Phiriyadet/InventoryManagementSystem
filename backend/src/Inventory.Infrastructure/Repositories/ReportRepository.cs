using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Reports.DTOs;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly InventoryDbContext _context;
    public ReportRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OverdueOrderReportItemDto>>
        GetOverdueOrdersAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken)
    {
        var result = await _context.OrderDetails.AsNoTracking()
    .Where(detail =>
        detail.OrdDate >= startDate &&
        detail.OrdDate < endDate &&
        detail.FinDate == null
    )
    .OrderBy(detail => detail.OrdDate)
    .ThenBy(detail => detail.Header.OrderNo).

            Select(detail => new OverdueOrderReportItemDto(
                0,
                detail.Header.Customer.CusId,
                detail.Header.Customer.CusName,
                detail.Goods.GoodsId,
                detail.Goods.GoodsName,
                detail.Header.OrderDate,
                detail.Header.OrderNo,
                detail.OrdDate,
                detail.Amount,
                detail.CostUnit,
                detail.TotPrc
            )
        ).ToListAsync(cancellationToken);

        return result;
    }
}

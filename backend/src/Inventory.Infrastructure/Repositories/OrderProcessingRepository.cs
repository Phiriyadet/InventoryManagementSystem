using Inventory.Application.Abstractions.Repositories;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class OrderProcessingRepository : IOrderProcessingRepository
{
    private readonly InventoryDbContext _context;

    public OrderProcessingRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<int> ProcessOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Get orders that have been delivered within date range
            var orders = await _context.OrderDetails
            .Where(d => d.FinDate != null
                    && d.FinDate >= startDate
                    && d.FinDate < endDate).Select(d => new
                    {
                        d.Header,
                        Detail = d
                    })
                    .ToListAsync(cancellationToken);

            // 2. No data to process
            if (orders.Count == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                return 0;
            }

            // 3. Map data from H_ORDER and D_ORDER to M_ORDER
            var masterOrders = orders.Select(x => new OrderMaster
            {
                CusId = x.Header.CusId,
                GoodsId = x.Detail.GoodsId,
                DocDate = x.Header.OrderDate,
                OrdDate = x.Detail.OrdDate,
                FinDate = x.Detail.FinDate!.Value,
                SysDate = DateTime.Now,
                Amount = x.Detail.Amount,
                CostTot = x.Detail.TotPrc
            }).ToList();

            // 4. Insert into M_ORDER
            await _context.OrderMasters.AddRangeAsync(
                masterOrders,
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            // 5. Delete processed details from D_ORDER
            var processedDetails = orders
                .Select(x => x.Detail)
                .ToList();

            _context.OrderDetails.RemoveRange(processedDetails);

            await _context.SaveChangesAsync(cancellationToken);

            // 6. Delete H_ORDER only when no details remain
            var affectedOrderNos = orders
                .Select(x => x.Header.OrderNo)
                .Distinct()
                .ToList();

            var headersToDelete = await _context.OrderHeaders
                .Where(header =>
                    affectedOrderNos.Contains(header.OrderNo)
                    && !_context.OrderDetails.Any(detail =>
                        detail.OrderNo == header.OrderNo))
                .ToListAsync(cancellationToken);

            _context.OrderHeaders.RemoveRange(headersToDelete);

            await _context.SaveChangesAsync(cancellationToken);

            // 7. Commit transaction
            await transaction.CommitAsync(cancellationToken);

            return orders.Count;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Orders.DTOs;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly InventoryDbContext _context;

    public OrderRepository(InventoryDbContext context)
    {
        _context = context;
    }

    // Order List
    public async Task<IReadOnlyList<OrderSummaryDto>> GetOrderListAsync(CancellationToken cancellationToken)
    {
        return await _context.OrderHeaders
            .AsNoTracking()
            .Select(h => new OrderSummaryDto(
                h.CusId,
                h.Customer.CusName,
                h.OrderNo,
                h.Details.Count(),
                h.Details.Sum(d => d.Amount)
            ))
            .ToListAsync(cancellationToken);
    }

    // H_ORDER
    public async Task<OrderHeader?> GetOrderByIdAsync(
        int orderNo,
        CancellationToken cancellationToken)
    {
        return await _context.OrderHeaders
            .AsNoTracking()
            .FirstOrDefaultAsync(
                h => h.OrderNo == orderNo,
                cancellationToken);
    }

    public async Task<OrderHeader> AddOrderAsync(
        OrderHeader order,
        CancellationToken cancellationToken)
    {
        await _context.OrderHeaders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return order;
    }

    public async Task<bool> DeleteOrderAsync(
        int orderNo,
        CancellationToken cancellationToken)
    {
        var order = await _context.OrderHeaders
            .FirstOrDefaultAsync(
                h => h.OrderNo == orderNo,
                cancellationToken);

        if (order is null)
            return false;

        _context.OrderHeaders.Remove(order);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    // D_ORDER
    public async Task<IReadOnlyList<OrderDetail>> GetOrderDetailsAsync(
        int orderNo,
        CancellationToken cancellationToken)
    {
        return await _context.OrderDetails
            .AsNoTracking()
            .Where(d => d.OrderNo == orderNo)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderDetail?> GetOrderDetailByIdAsync(
        int orderNo,
        string goodsId,
        CancellationToken cancellationToken)
    {
        return await _context.OrderDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(
                d => d.OrderNo == orderNo &&
                     d.GoodsId == goodsId,
                cancellationToken);
    }

    public async Task<OrderDetail> AddOrderDetailAsync(
        OrderDetail detail,
        CancellationToken cancellationToken)
    {
        await _context.OrderDetails.AddAsync(detail, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return detail;
    }

    public async Task<bool> UpdateOrderDetailAsync(
        OrderDetail detail,
        CancellationToken cancellationToken)
    {
        var existing = await _context.OrderDetails
            .FirstOrDefaultAsync(
                d => d.OrderNo == detail.OrderNo &&
                     d.GoodsId == detail.GoodsId,
                cancellationToken);

        if (existing is null)
            return false;

        existing.OrdDate = detail.OrdDate;
        existing.FinDate = detail.FinDate;
        existing.Amount = detail.Amount;
        existing.CostUnit = detail.CostUnit;
        existing.TotPrc = detail.TotPrc;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteOrderDetailAsync(
        int orderNo,
        string goodsId,
        CancellationToken cancellationToken)
    {
        var detail = await _context.OrderDetails
            .FirstOrDefaultAsync(
                d => d.OrderNo == orderNo &&
                     d.GoodsId == goodsId,
                cancellationToken);

        if (detail is null)
            return false;

        _context.OrderDetails.Remove(detail);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ReplaceOrderDetailAsync(
        OrderDetail oldEntity,
        OrderDetail newEntity,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            _context.OrderDetails.Add(newEntity);
            _context.OrderDetails.Remove(oldEntity);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

}
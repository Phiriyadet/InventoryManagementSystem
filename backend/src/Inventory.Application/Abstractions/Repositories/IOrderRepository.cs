using Inventory.Application.Orders.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    // Order List
    Task<IReadOnlyList<OrderSummaryDto>> GetOrderListAsync(
        CancellationToken cancellationToken);

    // H_ORDER
    Task<OrderHeader?> GetOrderByIdAsync(
        int orderNo,
        CancellationToken cancellationToken);

    Task<OrderHeader> AddOrderAsync(
        OrderHeader order,
        CancellationToken cancellationToken);

    Task<bool> DeleteOrderAsync(
        int orderNo,
        CancellationToken cancellationToken);

    // D_ORDER
    Task<IReadOnlyList<OrderDetail>> GetOrderDetailsAsync(
        int orderNo,
        CancellationToken cancellationToken);

    Task<OrderDetail?> GetOrderDetailByIdAsync(
        int orderNo,
        string goodsId,
        CancellationToken cancellationToken);

    Task<bool> ReplaceOrderDetailAsync(
        OrderDetail oldEntity,
        OrderDetail newEntity,
        CancellationToken cancellationToken);

    Task<OrderDetail> AddOrderDetailAsync(
        OrderDetail detail,
        CancellationToken cancellationToken);

    Task<bool> UpdateOrderDetailAsync(
        OrderDetail detail,
        CancellationToken cancellationToken);

    Task<bool> DeleteOrderDetailAsync(
        int orderNo,
        string goodsId,
        CancellationToken cancellationToken);
}
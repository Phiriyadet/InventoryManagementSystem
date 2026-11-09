using Inventory.Application.Common;
using Inventory.Application.Orders.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Services;

public interface IOrderService
{
    // Order List
    Task<Result<IEnumerable<OrderSummaryDto>>> GetOrderListAsync(
        CancellationToken cancellationToken);


    // H_ORDER
    Task<Result<OrderHeader>> GetOrderByIdAsync(
        int orderNo,
        CancellationToken cancellationToken);

    Task<Result<OrderHeader>> CreateOrderAsync(
        CreateHOrderRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeleteOrderAsync(
        int orderNo,
        CancellationToken cancellationToken);


    // D_ORDER
    Task<Result<IEnumerable<OrderDetail>>> GetOrderDetailsAsync(
        int orderNo,
        CancellationToken cancellationToken);

    Task<Result<OrderDetail>> GetOrderDetailByIdAsync(
        int orderNo,
        string goodsId,
        CancellationToken cancellationToken);

    Task<Result<OrderDetail>> AddOrderDetailAsync(
        CreateDOrderRequest request,
        CancellationToken cancellationToken);

    Task<Result<OrderDetail>> UpdateOrderDetailAsync(
        int orderNo,
        string goodsId,
        UpdateDOrderRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeleteOrderDetailAsync(
        int orderNo,
        string goodsId,
        CancellationToken cancellationToken);
}
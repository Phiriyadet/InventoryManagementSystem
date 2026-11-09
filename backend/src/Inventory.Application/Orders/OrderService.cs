using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;
using Inventory.Application.Common.Errors;
using Inventory.Application.Orders.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGoodsRepository _goodsRepository;
    private readonly ICustomerRepository _customerRepository;
    public OrderService(
        IOrderRepository orderRepository,
        IGoodsRepository goodsRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _goodsRepository = goodsRepository;
        _customerRepository = customerRepository;
    }

    private static readonly DateTime DefaultFinDate =
    new(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

    public async Task<Result<OrderDetail>> AddOrderDetailAsync(
    CreateDOrderRequest request,
    CancellationToken cancellationToken)
    {
        // Validate Amount
        if (request.Amount <= 0 || request.Amount > 9_999_999.99m)
            return Result<OrderDetail>.Failure(OrderErrors.InvalidAmount);

        // Validate OrdDate
        if (request.OrdDate == default)
            return Result<OrderDetail>.Failure(OrderErrors.InvalidOrdDate);

        // Validate Order
        var order = await _orderRepository.GetOrderByIdAsync(
            request.OrderNo, cancellationToken);

        if (order is null)
            return Result<OrderDetail>.Failure(OrderErrors.NotFound(request.OrderNo));

        // OrdDate ต้องไม่ก่อน OrderDate
        if (request.OrdDate.Date < order.OrderDate.Date)
            return Result<OrderDetail>.Failure(OrderErrors.InvalidOrdDate);

        // Validate FinDate (ถ้ามีการระบุ)
        if (request.FinDate.HasValue &&
            request.FinDate.Value.Date < request.OrdDate.Date)
            return Result<OrderDetail>.Failure(OrderErrors.InvalidFinDate);

        // Validate Goods
        var goods = await _goodsRepository.GetByIdAsync(
            request.GoodsId, cancellationToken);

        if (goods is null)
            return Result<OrderDetail>.Failure(GoodsErrors.NotFound(request.GoodsId));

        var entity = new OrderDetail
        {
            OrderNo = request.OrderNo,
            GoodsId = request.GoodsId,
            OrdDate = request.OrdDate,
            FinDate = request.FinDate ?? DefaultFinDate,
            Amount = request.Amount,
            CostUnit = goods.CostUnit,
            TotPrc = request.Amount * goods.CostUnit
        };

        var result = await _orderRepository.AddOrderDetailAsync(
            entity, cancellationToken);

        return Result<OrderDetail>.Success(result);
    }

    public async Task<Result<OrderHeader>> CreateOrderAsync(CreateHOrderRequest request, CancellationToken cancellationToken)
    {
        // Validate OrderDate
        if (request.OrderDate == default)
            return Result<OrderHeader>.Failure(OrderErrors.InvalidOrderDate);

        // Validate Customer
        var customer = await _customerRepository.ExistsAsync(
            request.CusId, cancellationToken);

        if (!customer)
            return Result<OrderHeader>.Failure(
                CustomerErrors.NotFound(request.CusId));

        var entity = new OrderHeader
        {
            OrderDate = request.OrderDate,
            CusId = request.CusId
        };
        var result = await _orderRepository.AddOrderAsync(entity, cancellationToken);

        return Result<OrderHeader>.Success(result);
    }

    public async Task<Result> DeleteOrderAsync(int orderNo, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.DeleteOrderAsync(orderNo, cancellationToken);

        return result ? Result.Success() : Result.Failure(OrderErrors.NotFound(orderNo));
    }

    public async Task<Result> DeleteOrderDetailAsync(int orderNo, string goodsId, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.DeleteOrderDetailAsync(orderNo, goodsId, cancellationToken);

        return result ? Result.Success() : Result.Failure(OrderErrors.NotFound(orderNo, goodsId));
    }

    public async Task<Result<OrderHeader>> GetOrderByIdAsync(int orderNo, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.GetOrderByIdAsync(orderNo, cancellationToken);

        return result != null ? Result<OrderHeader>.Success(result) : Result<OrderHeader>.Failure(OrderErrors.NotFound(orderNo));
    }

    public async Task<Result<OrderDetail>> GetOrderDetailByIdAsync(int orderNo, string goodsId, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.GetOrderDetailByIdAsync(orderNo, goodsId, cancellationToken);

        return result != null ? Result<OrderDetail>.Success(result) : Result<OrderDetail>.Failure(OrderErrors.NotFound(orderNo, goodsId));
    }

    public async Task<Result<IEnumerable<OrderDetail>>> GetOrderDetailsAsync(int orderNo, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(
            orderNo, cancellationToken);

        if (order is null)
            return Result<IEnumerable<OrderDetail>>.Failure(OrderErrors.NotFound(orderNo));

        var result = await _orderRepository.GetOrderDetailsAsync(orderNo, cancellationToken);

        return Result<IEnumerable<OrderDetail>>.Success(result);
    }

    public async Task<Result<IEnumerable<OrderSummaryDto>>> GetOrderListAsync(CancellationToken cancellationToken)
    {
        var result = await _orderRepository.GetOrderListAsync(cancellationToken);

        return Result<IEnumerable<OrderSummaryDto>>.Success(result);
    }

    public async Task<Result<OrderDetail>> UpdateOrderDetailAsync(
    int orderNo,
    string goodsId,
    UpdateDOrderRequest request,
    CancellationToken cancellationToken)
    {
        if (request.Amount <= 0 || request.Amount > 9_999_999.99m)
            return Result<OrderDetail>.Failure(OrderErrors.InvalidAmount);
            
        // ตรวจสอบสินค้าใหม่
        var goods = await _goodsRepository.GetByIdAsync(
            request.GoodsId, cancellationToken);

        if (goods is null)
            return Result<OrderDetail>.Failure(
                GoodsErrors.NotFound(request.GoodsId));

        // ค้นหารายการเดิม
        var entity = await _orderRepository.GetOrderDetailByIdAsync(
            orderNo, goodsId, cancellationToken);

        if (entity is null)
            return Result<OrderDetail>.Failure(
                OrderErrors.NotFound(orderNo, goodsId));

        // กรณีเปลี่ยนสินค้า
        if (goodsId != request.GoodsId)
        {
            // ตรวจสอบว่ามีสินค้าใหม่ใน Order นี้แล้วหรือไม่
            var existing = await _orderRepository.GetOrderDetailByIdAsync(
                orderNo, request.GoodsId, cancellationToken);

            if (existing is not null)
                return Result<OrderDetail>.Failure(
                    OrderErrors.AlreadyExists(orderNo, request.GoodsId));

            var newEntity = new OrderDetail
            {
                OrderNo = orderNo,
                GoodsId = request.GoodsId,
                OrdDate = request.OrdDate,
                FinDate = request.FinDate ?? DefaultFinDate,
                Amount = request.Amount,
                CostUnit = goods.CostUnit,
                TotPrc = request.Amount * goods.CostUnit
            };

            // เพิ่มรายการใหม่และลบรายการเก่าภายใน Transaction
            var result = await _orderRepository.ReplaceOrderDetailAsync(entity, newEntity, cancellationToken);

            return result
                ? Result<OrderDetail>.Success(newEntity)
                : Result<OrderDetail>.Failure(
                    OrderErrors.NotFound(orderNo, goodsId));
        }

        // กรณีสินค้าเดิม Update Entity
        entity.OrdDate = request.OrdDate;
        entity.FinDate = request.FinDate ?? DefaultFinDate;
        entity.Amount = request.Amount;
        entity.CostUnit = goods.CostUnit;
        entity.TotPrc = request.Amount * goods.CostUnit;

        var updated = await _orderRepository.UpdateOrderDetailAsync(
            entity, cancellationToken);

        return updated
            ? Result<OrderDetail>.Success(entity)
            : Result<OrderDetail>.Failure(
                OrderErrors.NotFound(orderNo, goodsId));
    }
}

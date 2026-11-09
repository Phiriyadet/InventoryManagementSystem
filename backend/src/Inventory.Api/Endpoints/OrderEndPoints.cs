using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;
using Inventory.Application.Orders.DTOs;

namespace Inventory.Api.Endpoints;

public static class OrderEndPoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(
        this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/orders").WithTags("Orders");

        // Get Order List
        api.MapGet("/", async (
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetOrderListAsync(cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error.Description);
        })
        .WithName("GetOrderList")
        .WithSummary("Get all orders");

        // Get Order By Id
        api.MapGet("/{orderNo:int}", async (
            int orderNo,
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetOrderByIdAsync(
                orderNo, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.Error.Type switch
                {
                    ErrorType.NotFound => Results.NotFound(result.Error),
                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("GetOrderById")
        .WithSummary("Get an order by its order number");

        // Create Order
        api.MapPost("/", async (
            CreateHOrderRequest request,
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.CreateOrderAsync(
                request, cancellationToken);

            return result.IsSuccess
                ? Results.Created(
                    $"/api/orders/{result.Value.OrderNo}",
                    result.Value)
                : result.Error.Type switch
                {
                    ErrorType.Validation => Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            { result.Error.Code, [result.Error.Description] }
                        }),

                    ErrorType.NotFound => Results.NotFound(result.Error),

                    ErrorType.Conflict => Results.Conflict(result.Error),

                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("CreateOrder")
        .WithSummary("Create a new order");

        // Delete Order
        api.MapDelete("/{orderNo:int}", async (
            int orderNo,
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.DeleteOrderAsync(
                orderNo, cancellationToken);

            return result.IsSuccess
                ? Results.NoContent()
                : result.Error.Type switch
                {
                    ErrorType.NotFound => Results.NotFound(result.Error),
                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("DeleteOrder")
        .WithSummary("Delete an order by its order number");

        // Get Order Details
        api.MapGet("/{orderNo:int}/details", async (
            int orderNo,
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetOrderDetailsAsync(
                orderNo, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.Error.Type switch
                {
                    ErrorType.NotFound => Results.NotFound(result.Error),
                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("GetOrderDetails")
        .WithSummary("Get all detail lines of an order");

        // Get Order Detail By Id
        api.MapGet("/{orderNo:int}/details/{goodsId}", async (
            int orderNo,
            string goodsId,
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetOrderDetailByIdAsync(
                orderNo, goodsId, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.Error.Type switch
                {
                    ErrorType.NotFound => Results.NotFound(result.Error),
                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("GetOrderDetailById")
        .WithSummary("Get a detail line of an order by goods Id");

        // Add Order Detail
        api.MapPost("/details", async (
            CreateDOrderRequest request,
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.AddOrderDetailAsync(
                request, cancellationToken);

            return result.IsSuccess
                ? Results.Created(
                    $"/api/orders/{result.Value.OrderNo}/details/{result.Value.GoodsId}",
                    result.Value)
                : result.Error.Type switch
                {
                    ErrorType.Validation => Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            { result.Error.Code, [result.Error.Description] }
                        }),

                    ErrorType.NotFound => Results.NotFound(result.Error),

                    ErrorType.Conflict => Results.Conflict(result.Error),

                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("AddOrderDetail")
        .WithSummary("Add a detail line to an order");

        // Update Order Detail
        api.MapPut("/{orderNo:int}/details/{goodsId}", async (
            int orderNo,
            string goodsId,
            UpdateDOrderRequest request,
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.UpdateOrderDetailAsync(
                orderNo, goodsId, request, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.Error.Type switch
                {
                    ErrorType.Validation => Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            { result.Error.Code, [result.Error.Description] }
                        }),

                    ErrorType.NotFound => Results.NotFound(result.Error),

                    ErrorType.Conflict => Results.Conflict(result.Error),

                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("UpdateOrderDetail")
        .WithSummary("Update a detail line of an order by goods Id");

        // Delete Order Detail
        api.MapDelete("/{orderNo:int}/details/{goodsId}", async (
            int orderNo,
            string goodsId,
            IOrderService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.DeleteOrderDetailAsync(
                orderNo, goodsId, cancellationToken);

            return result.IsSuccess
                ? Results.NoContent()
                : result.Error.Type switch
                {
                    ErrorType.NotFound => Results.NotFound(result.Error),
                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("DeleteOrderDetail")
        .WithSummary("Delete a detail line of an order by goods Id");

        return app;
    }
}
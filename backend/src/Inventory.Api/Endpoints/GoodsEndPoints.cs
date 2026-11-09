using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;
using Inventory.Application.Products.DTOs;

namespace Inventory.Api.Endpoints;

public static class GoodsEndPoints
{
    public static IEndpointRouteBuilder MapGoodsEndpoints(
        this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/goods").WithTags("Goods");

        api.MapGet("/{id}", async (string id, IGoodsService goodsService, CancellationToken cancellationToken) =>
        {
            var result = await goodsService.GetGoodsByIdAsync(id, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.Error.Type switch
                {
                    ErrorType.NotFound => Results.NotFound(result.Error),
                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("GetGoodsById")
        .WithSummary("Get goods by its Id");

        // app.MapGet("/by-name/{name}", async (string name, IGoodsService goodsService, CancellationToken cancellationToken) =>
        // {
        //     var result = await goodsService.GetGoodsByNameAsync(name, cancellationToken);

        //     return Results.Ok(result.Value);

        // });

        api.MapGet("/", async (
        IGoodsService goodsService,
        CancellationToken cancellationToken) =>
        {
            var result = await goodsService.GetAllGoodsAsync(cancellationToken);

            return Results.Ok(result.Value);

        })
        .WithName("GetAllGoods")
        .WithSummary("Get all goods");

        api.MapPost("/", async (CreateGoodsRequest goods, IGoodsService goodsService, CancellationToken cancellationToken) =>
        {
            var result = await goodsService.AddGoodsAsync(goods, cancellationToken);
            return result.IsSuccess
            ? Results.Created($"/api/goods/{result.Value.GoodsId}", result.Value)
            : result.Error.Type switch
            {
                ErrorType.Validation => Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        { result.Error.Code, [result.Error.Description] }
                    }),

                ErrorType.Conflict => Results.Conflict(result.Error),

                _ => Results.Problem(result.Error.Description)
            };
        })
        .WithName("CreateGoods")
        .WithSummary("Create new goods");


        api.MapPut("/{id}", async (string id, UpdateGoodsRequest goods, IGoodsService goodsService, CancellationToken cancellationToken) =>
        {
            var result = await goodsService.UpdateGoodsAsync(id, goods, cancellationToken);

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

                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("UpdateGoods")
        .WithSummary("Update existing goods by its Id");

        api.MapDelete("/{id}", async (string id, IGoodsService goodsService, CancellationToken cancellationToken) =>
        {
            var result = await goodsService.DeleteGoodsAsync(id, cancellationToken);

            return result.IsSuccess
            ? Results.NoContent()
            : result.Error.Type switch
            {
                ErrorType.NotFound => Results.NotFound(result.Error),
                _ => Results.Problem(result.Error.Description)
            };
        })
        .WithName("DeleteGoods")
        .WithSummary("Delete goods by its Id");

        return app;
    }
}
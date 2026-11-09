using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;

namespace Inventory.Api.Endpoints;

public static class OrderProcessingEndpoints
{
    public static IEndpointRouteBuilder MapOrderProcessingEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/order-processing")
            .WithTags("Order Processing");

        group.MapPost("/process", async (
            DateOnly startDate,
            DateOnly endDate,
            IOrderProcessingService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.ProcessOrdersAsync(
                startDate,
                endDate,
                cancellationToken
            );

            if (result.IsSuccess)
                return Results.Ok(result.Value);

            return result.Error.Type switch
            {
                ErrorType.Validation =>
                    Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            {
                                result.Error.Code,
                                [result.Error.Description]
                            }
                        }),

                _ => Results.Problem(result.Error.Description)
            };
        })
        .WithName("ProcessOrders")
        .WithSummary("Process delivered orders into Master File");

        return app;
    }
}

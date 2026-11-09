using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;

namespace Inventory.Api.Endpoints;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports")
            .WithTags("Reports");

        group.MapGet("/overdue-orders", async (
            DateOnly startDate,
            DateOnly endDate,
            IReportService reportService,
            CancellationToken cancellationToken) =>
        {
            var result = await reportService.GetOverdueOrdersAsync(
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
        .WithName("GetOverdueOrders")
        .WithSummary("Get overdue order report");

        return app;
    }
}

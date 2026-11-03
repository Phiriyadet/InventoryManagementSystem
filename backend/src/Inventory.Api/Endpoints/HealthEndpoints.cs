using Inventory.Infrastructure.Persistence;

namespace Inventory.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(
        this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/health").WithTags("Health");
        
        api.MapGet("/", () => Results.Ok(new
        {
            status = "Healthy",
            service = "Inventory.Api"
        }))
        .WithName("GetHealth");

        api.MapGet("/database", async (
            InventoryDbContext dbContext) =>
        {
            var connected = await dbContext.Database.CanConnectAsync();

            return connected
                ? Results.Ok(new { status = "Connected" })
                : Results.Problem("Database connection failed.");
        }).WithName("GetDatabaseHealth");

        return app;
    }
}

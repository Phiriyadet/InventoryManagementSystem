namespace Inventory.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", () => Results.Ok(new
        {
            status = "Healthy",
            service = "Inventory.Api"
        }))
        .WithName("GetHealth")
        .WithTags("Health");

        return app;
    }
}

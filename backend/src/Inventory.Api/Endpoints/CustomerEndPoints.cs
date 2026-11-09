using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;
using Inventory.Application.Customers.DTOs;

namespace Inventory.Api.Endpoints;

public static class CustomerEndPoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(
        this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/customers").WithTags("Customer");

        api.MapGet("/{id}", async (string id, ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var result = await customerService.GetCustomerByIdAsync(id, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.Error.Type switch
                {
                    ErrorType.NotFound => Results.NotFound(result.Error),
                    _ => Results.Problem(result.Error.Description)
                };
        })
        .WithName("GetCustomerById")
        .WithSummary("Get a customer by its Id");

        // app.MapGet("/by-name/{name}", async (string name, ICustomerService customerService, CancellationToken cancellationToken) =>
        // {
        //     var result = await customerService.GetCustomerByNameAsync(name, cancellationToken);

        //     return Results.Ok(result.Value);

        // });

        api.MapGet("/", async (
        ICustomerService customerService,
        CancellationToken cancellationToken) =>
        {
            var result = await customerService.GetAllCustomerAsync(cancellationToken);

            return Results.Ok(result.Value);

        })
        .WithName("GetAllCustomers")
        .WithSummary("Get all customers");

        api.MapPost("/", async (CreateCustomerRequest customer, ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var result = await customerService.AddCustomerAsync(customer, cancellationToken);
            return result.IsSuccess
            ? Results.Created($"/api/customers/{result.Value.CusId}", result.Value)
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
        .WithName("CreateCustomer")
        .WithSummary("Create a new customer");


        api.MapPut("/{id}", async (string id, UpdateCustomerRequest customer, ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var result = await customerService.UpdateCustomerAsync(id, customer, cancellationToken);

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
        .WithName("UpdateCustomer")
        .WithSummary("Update an existing customer by its Id");

        api.MapDelete("/{id}", async (string id, ICustomerService customerService, CancellationToken cancellationToken) =>
        {
            var result = await customerService.DeleteCustomerAsync(id, cancellationToken);

            return result.IsSuccess
            ? Results.NoContent()
            : result.Error.Type switch
            {
                ErrorType.NotFound => Results.NotFound(result.Error),
                _ => Results.Problem(result.Error.Description)
            };
        })
        .WithName("DeleteCustomer")
        .WithSummary("Delete a customer by its Id");

        return app;
    }
}
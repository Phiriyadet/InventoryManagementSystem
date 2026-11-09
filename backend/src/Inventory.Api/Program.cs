using Inventory.Api.Endpoints;
using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Abstractions.Services;
using Inventory.Application.Customers;
using Inventory.Application.Orders;
using Inventory.Application.Processing;
using Inventory.Application.Products;
using Inventory.Application.Reports;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("InventoryDb")
    )
);

// Register services
builder.Services.AddScoped<IGoodsRepository, GoodsRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IOrderProcessingRepository, OrderProcessingRepository>();

builder.Services.AddScoped<IGoodsService, GoodsService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("Frontend");

app.MapHealthEndpoints();
app.MapGoodsEndpoints();
app.MapCustomerEndpoints();
app.MapOrderEndpoints();
app.MapReportEndpoints();
app.MapOrderProcessingEndpoints();

await app.RunAsync();

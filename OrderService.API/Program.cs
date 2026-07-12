global using FastEndpoints;
global using FluentValidation;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Services;
using OrderService.Services.GetOrders;

var builder = WebApplication.CreateBuilder();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var connectionString = builder.Configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddDbContext<OrderServiceDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IService<List<OrderService.Entities.Order>, string>, GetOrdersService>();

var app = builder.Build();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();
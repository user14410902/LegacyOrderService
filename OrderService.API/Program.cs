global using FastEndpoints;
global using FluentValidation;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Services.AddOrders.Display;
using OrderService.Services.Services;
using OrderService.UseCases.Implementations;

var builder = WebApplication.CreateBuilder();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var connectionString = builder.Configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddDbContext<OrderServiceDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<CacheProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICreateOrderForCustomer, CreateOrderForCustomer>();
builder.Services.AddScoped<IOrderRetrievalService, GetOrdersService>();
builder.Services.AddScoped<IAddOrderDisplay, ConsoleAddOrderDisplay>();
builder.Services.AddScoped<IOrdersCreationService, AddOrdersService>();

var app = builder.Build();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();
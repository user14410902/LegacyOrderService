using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Entities;
using OrderService.Services.AddOrders.Display;
using OrderService.Services.Services;
using OrderService.Services.Sources;
using OrderService.UseCases.Implementations;

namespace OrderService.Services.Tests.AddOrders;

public class AddOrderService_SingleOrder_Tests
{

    private ServiceProvider _serviceProvider;

    private CancellationToken _cancellationToken;

    [SetUp]
    public async Task Setup()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        _cancellationToken = cts.Token;

        var connectionString = "Data Source=order.db";
        _serviceProvider = new ServiceCollection()
    .AddDbContext<OrderServiceDbContext>(options =>
    options.UseSqlite(connectionString))
              .AddScoped<IProductRepository, ProductRepository>()
        .AddScoped<CacheProductRepository>()
              .AddScoped<IOrderRepository, OrderRepository>()
        .AddMemoryCache()
        .AddLogging()
    .BuildServiceProvider();

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
        await DbInitializer.Seed(context, _cancellationToken);
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null!;
    }

    [Test]
    public async Task TestFlowWithObjects()
    {
        using var scope = _serviceProvider.CreateScope();
        var cacheProductRepository = scope.ServiceProvider.GetRequiredService<CacheProductRepository>();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AddOrdersService>>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var customerName = "CustomerName";
        string productName = "Doohickey";
        var quantity = 1;
        var mockSource = NSubstitute.Substitute.For<IRowsSource>();
        mockSource.Rows.Returns(new List<OrderRow> {
            new OrderRow {
                CustomerName = customerName,
                ProductName = productName,
                Quantity =quantity }
        });

        var mockDisplay = NSubstitute.Substitute.For<IAddOrderDisplay>();

        var useCase = new CreateOrderForCustomer();

        var target = new AddOrdersService(logger,
        productRepository,
        orderRepository,
        useCase,
        mockDisplay);

        using var cts = new CancellationTokenSource();
        var actualResult = await target.ExecuteAsync(mockSource, cts.Token);

        Assert.That(actualResult.IsSuccess, Is.True, "Expecting IsSuccess to be true.");
        Assert.That(actualResult.Value, Is.True, "Expecting the result to be true.");
        Assert.That(actualResult.Error, Is.Null, "Expecting no errors.");

    }

    [Test]
    public async Task TestFlowWithMocks()
    {
        var mockLogger = NSubstitute.Substitute.For<ILogger<AddOrdersService>>();
        var mockProductRepository = NSubstitute.Substitute.For<IProductRepository>();
        var mockOrderRepository = NSubstitute.Substitute.For<IOrderRepository>();
        var mockSource = NSubstitute.Substitute.For<IRowsSource>();
        var mockDisplay = NSubstitute.Substitute.For<IAddOrderDisplay>();

        string productName = "product name";
        var product = new Product(Id: Guid.NewGuid(), Name: productName, Price: 1.0M);
        mockProductRepository.GetProductAsync(productName, Arg.Any<CancellationToken>())
        .Returns(product);

        var customerName = "CustomerName";
        var quantity = 1;
        mockSource.Rows.Returns(new List<OrderRow> {
            new OrderRow {
                CustomerName = customerName,
                ProductName = productName,
                Quantity =quantity }
        });

        var expectedGuid = Guid.NewGuid();
        mockOrderRepository.Add(Arg.Any<Order>());

        var useCase = new CreateOrderForCustomer();

        var target = new AddOrdersService(mockLogger,
        mockProductRepository,
        mockOrderRepository,
        useCase,
        mockDisplay);

        using var cts = new CancellationTokenSource();
        var actualGuid = await target.ExecuteAsync(mockSource, cts.Token);

        mockDisplay.Received(1).DisplayOrder(Arg.Any<Order>());

        mockOrderRepository.Received(1).Add(Arg.Any<Order>());
        await mockOrderRepository.Received(1).SaveAsync(Arg.Any<CancellationToken>());


    }
}

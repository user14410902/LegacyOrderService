using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Entities;
using OrderService.Services.AddOrder;
using OrderService.UseCases.Implementations;

namespace OrderService.Services.Tests.AddOrder;

public class AddOrderServiceTests
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

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AddOrderService>>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var customerName = "CustomerName";
        string productName = "Doohickey";
        var quantity = 1;
        var mockSources = NSubstitute.Substitute.For<IAddOrderSources>();
        mockSources.GetCustomerName().Returns(customerName);
        mockSources.GetProductName().Returns(productName);
        mockSources.GetQuantity().Returns(quantity);

        var mockDisplay = NSubstitute.Substitute.For<IAddOrderDisplay>();

        var useCase = new CreateOrderForCustomer();

        var target = new AddOrderService(logger,
        productRepository,
        orderRepository,
        mockSources,
        mockDisplay,
        useCase);

        using var cts = new CancellationTokenSource();
        var actualGuid = await target.ExecuteAsync(cts.Token);

        Assert.That(actualGuid.IsSuccess, Is.True, "Expecting the result to be true.");
        Assert.That(actualGuid.Value, Is.Not.EqualTo(Guid.Empty), "Actual Guid ID of the new row is not valid.");

    }

    [Test]
    public async Task TestFlowWithMocks()
    {
        var mockLogger = NSubstitute.Substitute.For<ILogger<AddOrderService>>();
        var mockProductRepository = NSubstitute.Substitute.For<IProductRepository>();
        var mockOrderRepository = NSubstitute.Substitute.For<IOrderRepository>();
        var mockSources = NSubstitute.Substitute.For<IAddOrderSources>();
        var mockDisplay = NSubstitute.Substitute.For<IAddOrderDisplay>();

        string productName = "product name";
        var product = new Product(Id: Guid.NewGuid(), Name: productName, Price: 1.0M);
        mockProductRepository.GetProductAsync(productName, Arg.Any<CancellationToken>())
        .Returns(product);

        var customerName = "CustomerName";
        var quantity = 1;
        mockSources.GetCustomerName().Returns(customerName);
        mockSources.GetProductName().Returns(productName);
        mockSources.GetQuantity().Returns(quantity);

        var expectedGuid = Guid.NewGuid();
        mockOrderRepository.AddAndSaveSingleAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>())
        .Returns(expectedGuid);

        var useCase = new CreateOrderForCustomer();

        var target = new AddOrderService(mockLogger,
        mockProductRepository,
        mockOrderRepository,
        mockSources,
        mockDisplay,
        useCase);

        using var cts = new CancellationTokenSource();
        var actualGuid = await target.ExecuteAsync(cts.Token);

        mockDisplay.Received(1).DisplayOrder(Arg.Any<Order>());

        await mockOrderRepository.Received(1).AddAndSaveSingleAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());

        Assert.That(actualGuid.Value, Is.EqualTo(expectedGuid));
    }
}

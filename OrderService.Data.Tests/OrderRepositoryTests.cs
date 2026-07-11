using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;

namespace OrderService.Data.Tests;

public class OrderRepositoryTests
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
.AddScoped<OrderRepository>()
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
  public async Task NullOrder()
  {
    using var scope = _serviceProvider.CreateScope();

    var orderRepository = scope.ServiceProvider.GetRequiredService<OrderRepository>();
    Assert.ThrowsAsync<NullReferenceException>(async () =>
    await orderRepository.Save(null!, _cancellationToken),
     "Expecting exception when order is null");

  }

  [TestCase("Doohickey", 8.75)]
  public async Task ExistantProductName(string expectedProductName, decimal expectedProductPrice)
  {
    using var scope = _serviceProvider.CreateScope();

    var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
    var product = await productRepository.GetProductAsync("Gadget", _cancellationToken);
    var order = new Entities.Order("Test Customer", product!, 1);
    var orderRepository = scope.ServiceProvider.GetRequiredService<OrderRepository>();
    var actualDBId = await orderRepository.Save(order, _cancellationToken);
    Assert.That(actualDBId, Is.Not.EqualTo(Guid.Empty), "Actual Guid ID of the new row is not valid.");
  }

}

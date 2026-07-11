using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;

namespace OrderService.Data.Tests;

public class OrderRepositoryTests
{

  private ServiceProvider _serviceProvider;

  private IProductRepository _productRepository;

  private OrderRepository _orderRepository;

  [SetUp]
  public async Task Setup()
  {
    var connectionString = "Data Source=order.db";
    _serviceProvider = new ServiceCollection()
.AddDbContext<OrderServiceDbContext>(options =>
options.UseSqlite(connectionString))
.BuildServiceProvider();



    using var scope = _serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
    await DbInitializer.Seed(context);

    _productRepository = new ProductRepository(_serviceProvider);
    _orderRepository = new OrderRepository(_serviceProvider);

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
    Assert.ThrowsAsync<NullReferenceException>(async () => await _orderRepository.Save(null!), "Expecting exception when order is null");

  }

  [TestCase("Doohickey", 8.75)]
  public async Task ExistantProductName(string expectedProductName, decimal expectedProductPrice)
  {
    var product = await _productRepository.GetProductAsync("Gadget");
    var order = new Entities.Order("Test Customer", product!, 1);
    var actualDBId = await _orderRepository.Save(order);
    Assert.That(actualDBId, Is.Not.EqualTo(Guid.Empty), "Actual Guid ID of the new row is not valid.");
  }

}

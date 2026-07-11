using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OrderService.Data.Interfaces;
using OrderService.Data.Models;
using OrderService.Data.Repositories;

namespace OrderService.Data.Tests;

public class CacheProductRepositoryTests
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
    .AddMemoryCache()
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
  public async Task NonExistantProductName()
  {
    using var scope = _serviceProvider.CreateScope();
    var cacheProductRepository = scope.ServiceProvider.GetRequiredService<CacheProductRepository>();

    var product = await cacheProductRepository.GetProductAsync("this should not be in the database", _cancellationToken);

    Assert.That(product, Is.Null, "Expected product to be null.");
  }

  [TestCase("Doohickey", 8.75)]
  public async Task ExistantProductName(string expectedProductName, decimal expectedProductPrice)
  {
    using var scope = _serviceProvider.CreateScope();
    var cacheProductRepository = scope.ServiceProvider.GetRequiredService<CacheProductRepository>();

    var product = await cacheProductRepository.GetProductAsync(expectedProductName, _cancellationToken);

    Assert.That(product, Is.Not.Null, "Expected product to be not null.");
    Assert.That(product.Name, Is.EqualTo(expectedProductName), $"Expected product name ({expectedProductName}) is incorrect.");
    Assert.That(product.Price, Is.EqualTo(expectedProductPrice), $"Expected product price ({expectedProductPrice}) is incorrect.");
  }

  [Test]
  public async Task EnsureUnderlyingRepositoryIsOnlyCalledOnce()
  {
    using var scope = _serviceProvider.CreateScope();
    var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

    var mockProductRepository = Substitute.For<IProductRepository>();

    var productName = "Gadget";
    mockProductRepository.GetProductAsync(productName, _cancellationToken).Returns(new OrderService.Entities.Product(Id: Guid.NewGuid(), Name: productName, Price: 1.0M));

    var cacheProductRepository = new CacheProductRepository(mockProductRepository, memoryCache);

    await cacheProductRepository.GetProductAsync(productName, _cancellationToken);
    await cacheProductRepository.GetProductAsync(productName, _cancellationToken);

    await mockProductRepository.Received(1).GetProductAsync(productName, _cancellationToken); //underlying repository should only be called once
  }

}

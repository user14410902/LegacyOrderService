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
  private MemoryCache _memoryCache;

  private IProductRepository _cacheProductRepository;

  [SetUp]
  public async Task Setup()
  {
    var connectionString = "Data Source=order.db";
    _serviceProvider = new ServiceCollection()
.AddDbContext<OrderServiceDbContext>(options =>
options.UseSqlite(connectionString))
.BuildServiceProvider();

    var cacheOptions = new MemoryCacheOptions();
    _memoryCache = new MemoryCache(cacheOptions);


    using var scope = _serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
    await DbInitializer.Seed(context);

    var productRepository = new ProductRepository(_serviceProvider);
    _cacheProductRepository = new CacheProductRepository(productRepository, _memoryCache);
  }

  [TearDown]
  public void TearDown()
  {
    _serviceProvider?.Dispose();
    _serviceProvider = null!;

    _memoryCache?.Dispose();
  }

  [Test]
  public async Task NonExistantProductName()
  {
    var product = await _cacheProductRepository.GetProductAsync("this should not be in the database");

    Assert.That(product, Is.Null, "Expected product to be null.");
  }

  [TestCase("Doohickey", 8.75)]
  public async Task ExistantProductName(string expectedProductName, decimal expectedProductPrice)
  {
    var product = await _cacheProductRepository.GetProductAsync(expectedProductName);

    Assert.That(product, Is.Not.Null, "Expected product to be not null.");
    Assert.That(product.Name, Is.EqualTo(expectedProductName), $"Expected product name ({expectedProductName}) is incorrect.");
    Assert.That(product.Price, Is.EqualTo(expectedProductPrice), $"Expected product price ({expectedProductPrice}) is incorrect.");
  }

  [Test]
  public async Task EnsureUnderlyingRepositoryIsOnlyCalledOnce()
  {
    var mockProductRepository = Substitute.For<IProductRepository>();

    var productName = "Gadget";
    mockProductRepository.GetProductAsync(productName).Returns(new OrderService.Entities.Product(Id: Guid.NewGuid(), Name: productName, Price: 1.0M));

    _cacheProductRepository = new CacheProductRepository(mockProductRepository, _memoryCache);

    await _cacheProductRepository.GetProductAsync(productName);
    await _cacheProductRepository.GetProductAsync(productName);

    await mockProductRepository.Received(1).GetProductAsync(productName); //underlying repository should only be called once
  }

}

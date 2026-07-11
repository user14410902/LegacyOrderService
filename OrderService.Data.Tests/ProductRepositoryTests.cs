using OrderService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Data.Interfaces;

namespace OrderService.Data.Tests;

public class ProductRepositoryTests
{

    private IProductRepository _productRepository;

    private ServiceProvider _serviceProvider;
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
        var product = await _productRepository.GetProductAsync("this should not be in the database");

        Assert.That(product, Is.Null, "Expected product to be null.");
    }

    [TestCase("Doohickey", 8.75)]
    public async Task ExistantProductName(string expectedProductName, decimal expectedProductPrice)
    {
        var product = await _productRepository.GetProductAsync(expectedProductName);

        Assert.That(product, Is.Not.Null, "Expected product to be not null.");
        Assert.That(product.Name, Is.EqualTo(expectedProductName), $"Expected product name ({expectedProductName}) is incorrect.");
        Assert.That(product.Price, Is.EqualTo(expectedProductPrice), $"Expected product price ({expectedProductPrice}) is incorrect.");
    }
}

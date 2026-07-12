using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.Data.Interfaces;
using OrderService.Entities;
using OrderService.Services.AddOrderCSV;
using OrderService.UseCases.Implementations;

namespace OrderService.Services.Tests.AddOrderCSV;

public class AddOrderCSVServiceTests
{

  private ServiceProvider _serviceProvider;

  private CancellationToken _cancellationToken;

  [SetUp]
  public async Task Setup()
  {
    CancellationTokenSource cts = new CancellationTokenSource();
    _cancellationToken = cts.Token;


    _serviceProvider = new ServiceCollection()
          .AddScoped<IProductRepository, TestProductRepository>()
          .AddScoped<IOrderRepository, TestOrderRepository>()
    .AddMemoryCache()
    .AddLogging()
.BuildServiceProvider();

    using var scope = _serviceProvider.CreateScope();

  }

  [TearDown]
  public void TearDown()
  {
    _serviceProvider?.Dispose();
    _serviceProvider = null!;
  }

  [TestCase("./sample_csv_files/sample_all_valid_orders.csv", 4, 0)]
  [TestCase("./sample_csv_files/sample_empty.csv", 0, 1)]
  [TestCase("./sample_csv_files/sample_missing_customer.csv", 3, 2)]
  [TestCase("./sample_csv_files/sample_missing_product.csv", 3, 2)]
  [TestCase("./sample_csv_files/sample_missing_quantity.csv", 3, 2)]
  [TestCase("./sample_csv_files/sample_negative_quantity.csv", 3, 2)]
  [TestCase("./sample_csv_files/sample_string_as_quantity.csv", 3, 2)]
  public async Task TestFlowWithObjects(string filename, int expectedOrders, int expectedErrors)
  {

    string testDirectory = TestContext.CurrentContext.TestDirectory;

    string filePath = Path.Combine(testDirectory, filename);

    using var scope = _serviceProvider.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AddOrderCSVService>>();
    var productRepository = new TestProductRepository();
    var orderRepository = new TestOrderRepository();
    var source = new CSVHelperRowSource(filePath);
    var useCase = new CreateOrderForCustomer();

    var target = new AddOrderCSVService(logger, productRepository, orderRepository, source, useCase);

    var result = await target.ExecuteAsync(_cancellationToken);

    Assert.That(orderRepository.Orders.Count, Is.EqualTo(expectedOrders), "Expected number of orders is incorrect.");
    if (expectedErrors == 0)
    {
      Assert.That(result.IsSuccess, Is.True);
    }
    else
    {
      Assert.That(result.IsSuccess, Is.False);
      Assert.That(result.Error!.Count, Is.EqualTo(expectedErrors), "Incorrect number of errors.");
    }

    if (expectedOrders > 0)
    {
      Assert.That(orderRepository.CountCall_SaveAsync, Is.EqualTo(1), "Expecting orders to be save to the OrderRepository.");
    }

  }

  private class TestProductRepository : IProductRepository
  {
    private Dictionary<string, Product> _products = new Dictionary<string, Product>
    {
      {"Gadget", new Product(Guid.NewGuid(), "Gadget",10M)},
      {"Widget", new Product(Guid.NewGuid(), "Widget",20M)},
      {"Doohickey", new Product(Guid.NewGuid(), "Doohickey",30M)},
      {"test", new Product(Guid.NewGuid(), "test",40M)},
    };
    public Task<Product?> GetProductAsync(string productName, CancellationToken cancellationToken)
    {
      _products.TryGetValue(productName, out var product);
      return Task.FromResult(product);
    }
  }

  // Using my own mock classes instead of NSubsitute because it is more intuitive. 
  // And to experiment with alternative mocking.

  private class TestOrderRepository : IOrderRepository
  {

    public List<Order> Orders { get; private set; } = new List<Order>();
    public int CountCall_SaveAsync { get; private set; } = 0;
    public void Add(Order entity)
    {
      Orders.Add(entity);
    }

    public Task<Guid> AddAndSaveSingleAsync(Order order, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SaveAsync(CancellationToken cancellationToken)
    {
      CountCall_SaveAsync++;
      return Task.CompletedTask;
    }
  }

  private class TestSource : ICSVRowSource
  {
    public IEnumerable<CSVOrderRow> Rows => throw new NotImplementedException();

    public string SourceDescription => throw new NotImplementedException();
  }
}

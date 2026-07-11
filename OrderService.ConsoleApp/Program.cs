using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.Data.Repositories;
using OrderService.UseCases;

namespace OrderService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();

            var logger = InitialiseAndReturnLogger(configuration);

            string connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' not found.");

            logger.LogInformation("Connection string: {ConnectionString}", connectionString);

            var serviceProvider = new ServiceCollection()
    .AddDbContext<OrderServiceDbContext>(options =>
        options.UseSqlite(connectionString))
        .AddMemoryCache()
    .BuildServiceProvider();

            await SeedDatabaseIfRequired(serviceProvider, logger);



            logger.LogInformation("Welcome to Order Processor!");
            logger.LogInformation("Enter customer name:");
            string? customerName = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.

            logger.LogInformation("Enter product name:");
            string productName = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.
            using var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var productRepository = new ProductRepository(serviceProvider); //TODO refactor 
            var cacheProductRepository = new CacheProductRepository(productRepository, memoryCache);
            var product = await productRepository.GetProductAsync(productName); //TODO what if product is null?
            if (product == null)
            {
                logger.LogError("Failed to retrieve {ProductName} from database. Stopping.", productName);
                return;
            }

            int qty = 0;
            bool quantityValid = false;
            while (!quantityValid)
            {
                logger.LogInformation("Enter quantity greater zero:");
                string qtyInput = Console.ReadLine() ?? "0";
                quantityValid = Int32.TryParse(qtyInput, out qty) && qty > 0;
                if (!quantityValid)
                {
                    logger.LogInformation("Quantity is invalud. The quantity must be a whole number greater than 0.");
                }
            }

            logger.LogInformation("Processing order...");

            var useCase = new CreateOrderForCustomer();
            var result = useCase.Execute(customerName, product, qty);

            if (result.IsSuccess)
            {
                var order = result.Value;
                logger.LogInformation("Order complete!");
                logger.LogInformation("Customer: " + order.CustomerName);
                logger.LogInformation("Product: " + order.Product.Name);
                logger.LogInformation("Quantity: " + order.Quantity);
                logger.LogInformation("Total: $" + order.Total);

                logger.LogInformation("Saving order to database...");
                var repo = new OrderRepository(serviceProvider);
                await repo.Save(order);
            }
            else
            {
                logger.LogError("An error occured while creating the error. Error message: {ErrorMessage}", result.Error);
            }
            logger.LogInformation("Done.");
        }

        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();
        }

        private static ILogger InitialiseAndReturnLogger(IConfigurationRoot configuration)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                // Bind the "Logging" section of appsettings.json to the logger configuration
                builder.AddConfiguration(configuration.GetSection("Logging"));

                // Add console logger
                builder.AddConsole();
            });

            return loggerFactory.CreateLogger<Program>();
        }

        private static async Task SeedDatabaseIfRequired(ServiceProvider serviceProvider, ILogger logger)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();

            logger.LogInformation("Connection string: {ConnectionString}", context.Database.GetConnectionString());
            await DbInitializer.Seed(context);
        }
    }
}

using LegacyOrderService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.UseCases;

namespace LegacyOrderService
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
    .BuildServiceProvider();

            await SeedDatabaseIfRequired(serviceProvider, logger);



            logger.LogInformation("Welcome to Order Processor!");
            logger.LogInformation("Enter customer name:");
            string? customerName = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.

            logger.LogInformation("Enter product name:");
            string productName = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.
            var productRepo = new ProductRepository(serviceProvider);
            var product = await productRepo.GetProduct(productName);


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
            var order = useCase.Execute(customerName, product, qty);


            logger.LogInformation("Order complete!");
            logger.LogInformation("Customer: " + order.CustomerName);
            logger.LogInformation("Product: " + order.Product.Name);
            logger.LogInformation("Quantity: " + order.Quantity);
            logger.LogInformation("Total: $" + order.Total);

            logger.LogInformation("Saving order to database...");
            var repo = new OrderRepository(serviceProvider);
            await repo.Save(order);
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

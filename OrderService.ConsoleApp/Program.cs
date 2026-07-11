using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;
using OrderService.UseCases;

namespace OrderService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new HostApplicationBuilderSettings
            {
                // Sets the content root to the directory where the app executable/assembly is located
                ContentRootPath = AppContext.BaseDirectory,
                Args = args
            };
            var builder = Host.CreateApplicationBuilder(settings);

            string connectionString = builder.Configuration.GetConnectionString("Default")
                      ?? throw new InvalidOperationException("Connection string 'Default' not found.");

            builder.Services.AddDbContext<OrderServiceDbContext>(options => options.UseSqlite(connectionString));
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<CacheProductRepository>();
            builder.Services.AddScoped<OrderRepository>();

            using IHost host = builder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Welcome to Order Processor!");
            logger.LogInformation("Connection string: {ConnectionString}", connectionString);

            using (IServiceScope scope = host.Services.CreateScope())
            {

                using var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                await SeedDatabaseIfRequired(scope, logger, cancellationToken);

                logger.LogInformation("Enter customer name:");
                string? customerName = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.

                logger.LogInformation("Enter product name:");
                string productName = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.
                var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                var product = await productRepository.GetProductAsync(productName, cancellationToken); //TODO what if product is null?
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
                    var orderRepository = scope.ServiceProvider.GetRequiredService<OrderRepository>();
                    await orderRepository.Save(order, cancellationToken);
                }
                else
                {
                    logger.LogError("An error occured while creating the error. Error message: {ErrorMessage}", result.Error);
                }
                logger.LogInformation("Done.");
            }
        }

        private static async Task SeedDatabaseIfRequired(IServiceScope scope, ILogger logger, CancellationToken cancellationToken)
        {
            logger.LogInformation("Seeding database if required...");
            var context = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
            logger.LogInformation("Connection string: {ConnectionString}", context.Database.GetConnectionString());
            await DbInitializer.Seed(context, cancellationToken);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Services;
using OrderService.Services.AddOrder;
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
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IAddOrderSources, ConsoleAddOrderSources>();
            builder.Services.AddScoped<IAddOrderDisplay, ConsoleAddOrderDisplay>();
            builder.Services.AddScoped<AddOrderService>();

            using IHost host = builder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Welcome to Order Processor!");
            logger.LogInformation("Connection string: {ConnectionString}", connectionString);

            using (IServiceScope scope = host.Services.CreateScope())
            {
                using var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                await SeedDatabaseIfRequired(scope, logger, cancellationToken);

                var service = scope.ServiceProvider.GetRequiredService<AddOrderService>();
                await service.ExecuteAsync(cancellationToken);
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

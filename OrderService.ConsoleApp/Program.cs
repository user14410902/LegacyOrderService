using System.CommandLine;
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
        static async Task<int> Main(string[] args)
        {


            //---------------------------------------------------------------------
            var verboseOption = new Option<bool>("--verbose", "-v")
            {
                Description = "Enable verbose console logging."
            };


            var customerNameArgument = new Argument<string>("customerName")
            {
                Description = "Customer Name"
            };
            var productNameArgument = new Argument<string>("productName")
            {
                Description = "Product Name"
            };

            var quantityArgument = new Argument<int>("quantity")
            {
                Description = "Quantity",
            };
            quantityArgument.Validators.Add(result =>
            {
                int value = result.GetValue<int>("quantity");//GetValueForArgument(ageArgument);

                if (value <= 0)
                {
                    result.AddError("Quantity must be a whole number greater or equal to 1.");
                }
            });
            var addOrderCommand = new Command("addOrder", "Add a new order")
            {
                Arguments = { customerNameArgument, productNameArgument, quantityArgument }
            };
            var addOrderInteractiveCommand = new Command("addOrderInteractive", "Add a new order using the console");

            var rootCommand = new RootCommand("Welcome to Order Processor!")
            {
                Options = { verboseOption }
            };
            rootCommand.Subcommands.Add(addOrderCommand);
            rootCommand.Subcommands.Add(addOrderInteractiveCommand);

            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            addOrderCommand.SetAction(async parseResult =>
                {
                    var customerName = parseResult.GetValue(customerNameArgument)!;
                    var productName = parseResult.GetValue(productNameArgument)!;
                    var quantity = parseResult.GetValue(quantityArgument)!;
                    var verbose = parseResult.GetValue(verboseOption);
                    var host = BuildHost(args, isInteractive: false, verbose,
                customerName, productName, quantity);
                    await ExecuteAsync(host, cancellationToken);
                });

            addOrderInteractiveCommand.SetAction(async parseResult =>
            {
                var verbose = parseResult.GetValue(verboseOption);
                var host = BuildHost(args, isInteractive: true, verbose);
                await ExecuteAsync(host, cancellationToken);
            });

            return rootCommand.Parse(args).Invoke();
        }

        private static async Task SeedDatabaseIfRequired(IServiceScope scope, ILogger logger, CancellationToken cancellationToken)
        {
            logger.LogInformation("Seeding database if required...");
            var context = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
            logger.LogInformation("Connection string: {ConnectionString}", context.Database.GetConnectionString());
            await DbInitializer.Seed(context, cancellationToken);
        }

        private static IHost BuildHost(string[] args, bool isInteractive, bool verbose,
        string customerName = null!, string productName = null!, int quantity = 0)
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

            builder.Logging.SetMinimumLevel(verbose ? LogLevel.Debug : LogLevel.Warning);
            builder.Services.AddDbContext<OrderServiceDbContext>(options => options.UseSqlite(connectionString));
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<CacheProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            if (isInteractive)
            {
                builder.Services.AddScoped<IAddOrderSources, ConsoleAddOrderSources>();
            }
            else
            {
                builder.Services.AddScoped<IAddOrderSources, CommanLineAddOrderSources>(
                    x => new CommanLineAddOrderSources(customerName, productName, quantity));
            }
            builder.Services.AddScoped<IAddOrderDisplay, ConsoleAddOrderDisplay>();
            builder.Services.AddScoped<AddOrderService>();

            return builder.Build();
        }

        private static async Task ExecuteAsync(IHost host, CancellationToken cancellationToken)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();

                await SeedDatabaseIfRequired(scope, logger, cancellationToken);

                var service = scope.ServiceProvider.GetRequiredService<AddOrderService>();
                await service.ExecuteAsync(cancellationToken);
            }
        }
    }
}

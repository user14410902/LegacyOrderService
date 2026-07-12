using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Common;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Services;
using OrderService.Services.AddOrder;
using OrderService.Services.AddOrderCSV;
using OrderService.UseCases;
using OrderService.UseCases.Implementations;
using OrderService.Services.GetOrders;

//TODO Revisite the Result<...> aliases used for the IService implementations.
using ResultSingleOrder = OrderService.Common.Result<System.Guid, string>;
using ResultCSV = OrderService.Common.Result<bool, System.Collections.Generic.List<string>>;
using ResultGetOrders = OrderService.Common.Result<System.Collections.Generic.List<OrderService.Entities.Order>, string>;

namespace OrderService
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = BuildRootCommand(args);
            return rootCommand.Parse(args).Invoke();
        }

        private static async Task SeedDatabaseIfRequired(IServiceScope scope, ILogger logger, CancellationToken cancellationToken)
        {
            logger.LogInformation("Seeding database if required...");
            var context = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
            logger.LogInformation("Connection string: {ConnectionString}", context.Database.GetConnectionString());
            await DbInitializer.Seed(context, cancellationToken);
        }

        private static HostApplicationBuilder BuildHostBase(string[] args, bool verbose)
        {
            var builder = Host.CreateApplicationBuilder(GetHostBuilderSettings(args));

            builder.Logging.SetMinimumLevel(verbose ? LogLevel.Debug : LogLevel.Warning);
            builder.Services.AddDbContext<OrderServiceDbContext>(options => options.UseSqlite(GetConnectionString(builder)));
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<CacheProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICreateOrderForCustomer, CreateOrderForCustomer>();

            return builder;
        }

        private static IHost BuildHostInteractive(string[] arg, bool verbose)
        {
            var builder = BuildHostBase(arg, verbose);

            builder.Services.AddScoped<IAddOrderSources, ConsoleAddOrderSources>()
            .AddScoped<IAddOrderDisplay, ConsoleAddOrderDisplay>()
            .AddScoped<ICreateOrderForCustomer, CreateOrderForCustomer>()
            .AddScoped<IService<System.Guid, string>, AddOrderService>();

            return builder.Build();
        }

        private static IHost BuildHostPassInValues(string[] arg, bool verbose,
        string customerName, string productName, int quantity)
        {
            var builder = BuildHostBase(arg, verbose);

            builder.Services.AddScoped<IAddOrderSources, CommanLineAddOrderSources>(
                    x => new CommanLineAddOrderSources(customerName, productName, quantity))
            .AddScoped<IAddOrderDisplay, ConsoleAddOrderDisplay>()
            .AddScoped<ICreateOrderForCustomer, CreateOrderForCustomer>()
            .AddScoped<AddOrderService>();

            return builder.Build();
        }

        private static IHost BuilderHostCSV(string[] arg, bool verbose, FileInfo file)
        {
            var builder = BuildHostBase(arg, verbose);

            builder.Services.AddScoped<IService<bool, List<string>>, AddOrderCSVService>();
            builder.Services.AddScoped<ICSVRowSource,
             CSVHelperRowSource>(x => new CSVHelperRowSource(file.FullName));

            return builder.Build();

        }

        private static async Task ExecuteAsync<ResultType, ErrorType>(IHost host, Action<ILogger, Result<ResultType, ErrorType>> finalAction, CancellationToken cancellationToken)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();

                await SeedDatabaseIfRequired(scope, logger, cancellationToken);

                var service = scope.ServiceProvider.GetRequiredService<IService<ResultType, ErrorType>>();
                var result = await service.ExecuteAsync(cancellationToken);
                finalAction(logger, result);
            }
        }

        private static RootCommand BuildRootCommand(string[] args)
        {
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
                int value = result.GetValue<int>("quantity");

                if (value <= 0)
                {
                    result.AddError("Quantity must be a whole number greater or equal to 1.");
                }
            });

            var filenameArgument = new Argument<FileInfo>("filename")
            {
                Description = "Complete path and filename of the CSV file"
            };
            filenameArgument.Validators.Add(result =>
            {
                var file = result.GetValue<FileInfo>("filename");
                if (file == null)
                {
                    result.AddError("Input file invalid.");
                    return;
                }
                var fullname = file.FullName;

                if (string.IsNullOrWhiteSpace(fullname) ||
                fullname.IndexOfAny(Path.GetInvalidFileNameChars()) > 0 ||
                 !File.Exists(fullname))
                {
                    result.AddError("Filename does not appear to be valid and/or the file does not exist.");
                }
            });

            var addOrderCommand = new Command("addOrder", "Add a new order")
            {
                Arguments = { customerNameArgument, productNameArgument, quantityArgument },
                Options = { verboseOption }
            };
            var addOrderInteractiveCommand = new Command("addOrderInteractive",
             "Add a new order using the console")
            {
                Options = { verboseOption }
            };

            var addOrderCSVFileCommand = new Command("addOrderCSV", "Import orders through a CSV file.")
            {
                Arguments = { filenameArgument },
                Options = { verboseOption }
            };

            var showAllOrdersCommand = new Command("showAllOrders", "List all the orders on the command line.")
            {
                Options = { verboseOption }
            };

            var rootCommand = new RootCommand("Welcome to Order Processor!")
            {
                Options = { verboseOption }
            };
            rootCommand.Subcommands.Add(addOrderCommand);
            rootCommand.Subcommands.Add(addOrderInteractiveCommand);
            rootCommand.Subcommands.Add(addOrderCSVFileCommand);
            rootCommand.Subcommands.Add(showAllOrdersCommand);

            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            addOrderCommand.SetAction(async parseResult =>
                {
                    var customerName = parseResult.GetValue(customerNameArgument)!;
                    var productName = parseResult.GetValue(productNameArgument)!;
                    var quantity = parseResult.GetValue(quantityArgument)!;
                    var verbose = parseResult.GetValue(verboseOption);
                    var host = BuildHostPassInValues(args, verbose,
                customerName, productName, quantity);
                    await ExecuteAsync<Guid, String>(host, LogResult, cancellationToken);
                });

            addOrderInteractiveCommand.SetAction(async parseResult =>
            {
                var verbose = parseResult.GetValue(verboseOption);
                var host = BuildHostInteractive(args, verbose);
                await ExecuteAsync<Guid, String>(host, LogResult, cancellationToken);
            });

            addOrderCSVFileCommand.SetAction(async parseResult =>
            {
                var verbose = parseResult.GetValue(verboseOption);
                var file = parseResult.GetValue(filenameArgument)!;
                var host = BuilderHostCSV(args, verbose, file);
                await ExecuteAsync<bool, List<string>>(host, LogResult, cancellationToken);
            });

            showAllOrdersCommand.SetAction(async parseResult =>
            {
                //TODO Maybe refactor here to avoid some of the code duplication.
                var builder = Host.CreateApplicationBuilder(GetHostBuilderSettings(args));
                var verbose = parseResult.GetValue(verboseOption);
                builder.Logging.SetMinimumLevel(verbose ? LogLevel.Debug : LogLevel.Warning);
                builder.Services.AddDbContext<OrderServiceDbContext>(options => options.UseSqlite(GetConnectionString(builder)));
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IService<List<Entities.Order>, string>, GetOrdersService>();

                var host = builder.Build();

                await ExecuteAsync<List<Entities.Order>, string>(host, LogResult, cancellationToken);

            });

            return rootCommand;
        }

        private static HostApplicationBuilderSettings GetHostBuilderSettings(string[] args)
        {
            return new HostApplicationBuilderSettings
            {
                // Sets the content root to the directory where the app executable/assembly is located to be able to load the appsettings.json
                ContentRootPath = AppContext.BaseDirectory,
                Args = args
            };
        }

        private static string GetConnectionString(HostApplicationBuilder builder)
        {
            return builder.Configuration.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        }

        //TODO Move LogResult methods somewhere else because the Program class is getting too big?
        private static void LogResult(ILogger logger, ResultSingleOrder result)
        {
            if (result.IsSuccess)
            {
                logger.LogInformation($"Order created sucessfully. New order ID: {result.Value}");
            }
            else
            {
                logger.LogError(result.Error);
            }
        }

        private static void LogResult(ILogger logger, ResultCSV result)
        {
            if (result.IsSuccess)
            {
                logger.LogInformation("CSV file processed successfully.");
            }
            else
            {
                foreach (var error in result.Error!)
                {
                    logger.LogError("{error}", error);
                }
            }
        }

        private static void LogResult(ILogger logger, ResultGetOrders result)
        {
            if (result.IsSuccess)
            {
                foreach (var order in result.Value)
                {
                    Console.WriteLine(order.ToString()); //yes, not using logger
                }
            }
            else
            {
                Console.WriteLine(result.Error); //yes, not using logger
            }
        }

    }
}

using LegacyOrderService.Data;
using Microsoft.Extensions.Logging;
using OrderService.UseCases;

namespace LegacyOrderService
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = InitialiseAndGetLogger();

            logger.LogInformation("Welcome to Order Processor!");
            logger.LogInformation("Enter customer name:");
            string? customerName = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.

            logger.LogInformation("Enter product name:");
            string productName = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.
            var productRepo = new ProductRepository();
            var product = productRepo.GetProduct(productName);


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
            var repo = new OrderRepository();
            repo.Save(order);
            logger.LogInformation("Done.");
        }

        private static ILogger InitialiseAndGetLogger()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

            return loggerFactory.CreateLogger<Program>();
        }
    }
}

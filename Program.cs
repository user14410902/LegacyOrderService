using LegacyOrderService.Models;
using LegacyOrderService.Data;
using Microsoft.Extensions.Logging;

namespace LegacyOrderService
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = InitialiseAndGetLogger();

            logger.LogInformation("Welcome to Order Processor!");
            logger.LogInformation("Enter customer name:");
            string name = Console.ReadLine() ?? string.Empty;

            logger.LogInformation("Enter product name:");
            string product = Console.ReadLine() ?? string.Empty;
            var productRepo = new ProductRepository();
            double price = productRepo.GetPrice(product);

            int qty = 0;
            bool quantityValid = false;
            while (!quantityValid)
            {
                logger.LogInformation("Enter quantity greater zero:");
                string qtyInput = Console.ReadLine() ?? "0";
                quantityValid = Int32.TryParse(qtyInput, out qty) && qty > 0;
                if (!quantityValid)
                {
                    logger.LogError("Quantity is invalud. The quantity must be a whole number greater than 0.");
                }
            }

            logger.LogInformation("Processing order...");

            var order = new Order(CustomerName: name,
                ProductName: product,
                Quantity: qty,
                Price: price //we are storing the unit price
            );

            double total = order.Quantity * order.Price;

            logger.LogInformation("Order complete!");
            logger.LogInformation("Customer: " + order.CustomerName);
            logger.LogInformation("Product: " + order.ProductName);
            logger.LogInformation("Quantity: " + order.Quantity);
            logger.LogInformation("Total: $" + total); //we are displaying the total price

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

using System;
using LegacyOrderService.Models;
using LegacyOrderService.Data;

namespace LegacyOrderService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Order Processor!");
            Console.WriteLine("Enter customer name:");
            string? name = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.

            Console.WriteLine("Enter product name:");
            string product = Console.ReadLine() ?? string.Empty; //TODO Improve on validation.
            var productRepo = new ProductRepository();
            double price = productRepo.GetPrice(product);


            int qty = 0;
            bool quantityValid = false;
            while (!quantityValid)
            {
                Console.WriteLine("Enter quantity greater zero:");
                string qtyInput = Console.ReadLine() ?? "0";
                quantityValid = Int32.TryParse(qtyInput, out qty) && qty > 0;
                if (!quantityValid)
                {
                    Console.WriteLine("Quantity is invalud. The quantity must be a whole number greater than 0.");
                }
            }

            Console.WriteLine("Processing order...");

            Order order = new Order(
            CustomerName: name,
            ProductName: product,
            Quantity: qty,
            Price: price); //storing unit price in repository!

            double total = order.Quantity * order.Price;

            Console.WriteLine("Order complete!");
            Console.WriteLine("Customer: " + order.CustomerName);
            Console.WriteLine("Product: " + order.ProductName);
            Console.WriteLine("Quantity: " + order.Quantity);
            Console.WriteLine("Total: $" + total); //displaying total price!

            Console.WriteLine("Saving order to database...");
            var repo = new OrderRepository();
            repo.Save(order);
            Console.WriteLine("Done.");
        }
    }
}

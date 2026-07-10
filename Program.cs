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
            string name = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter product name:");
            string product = Console.ReadLine() ?? string.Empty;
            var productRepo = new ProductRepository();
            double price = productRepo.GetPrice(product);


            Console.WriteLine("Enter quantity:");
            string qtyInput = Console.ReadLine() ?? "0";
            int qty = Convert.ToInt32(qtyInput);

            Console.WriteLine("Processing order...");

            Order order = new()
            {
                CustomerName = name,
                ProductName = product,
                Quantity = qty,
                Price = price //we are storing the unit price
            };

            double total = order.Quantity * order.Price;

            Console.WriteLine("Order complete!");
            Console.WriteLine("Customer: " + order.CustomerName);
            Console.WriteLine("Product: " + order.ProductName);
            Console.WriteLine("Quantity: " + order.Quantity);
            Console.WriteLine("Total: $" + total); //we are displaying the total price

            Console.WriteLine("Saving order to database...");
            var repo = new OrderRepository();
            repo.Save(order);
            Console.WriteLine("Done.");
        }
    }
}

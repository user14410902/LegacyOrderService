using Microsoft.Data.Sqlite;
using OrderService.Entities;

namespace LegacyOrderService.Data
{
    public class OrderRepository
    {
        private string _connectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "orders.db")}";


        public void Save(Order order)
        {
            var connection = new SqliteConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $@"
                INSERT INTO Orders (CustomerName, ProductName, Quantity, Price)
                VALUES ('{order.CustomerName}', '{order.Product.Name}', {order.Quantity}, {order.Product.Price})";

            command.ExecuteNonQuery();
        }
    }
}

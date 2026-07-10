namespace LegacyOrderService.Models
{
    public class Order
    {
        public required string CustomerName;
        public required string ProductName;
        public int Quantity;
        public double Price;
    }
}

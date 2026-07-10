namespace LegacyOrderService.Models
{
    public record Order(string CustomerName,
    string ProductName,
    int Quantity,
    double Price);

}

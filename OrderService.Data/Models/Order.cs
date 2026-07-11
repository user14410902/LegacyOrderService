using System.ComponentModel.DataAnnotations;

namespace OrderService.Data.Models;

public class Order
{
  public Guid Id { get; set; }

  public required string CustomerName { get; set; }

  public required Guid ProductId { get; set; }
  public Product? Product { get; set; }

  public int Quantity { get; set; }

}
using System.ComponentModel.DataAnnotations;

namespace OrderService.Data.Models;

public class Order
{
  //TODO Is Guid necessary for this local-only app? Then again, the company is expecting growth :)
  public Guid Id { get; set; }

  public required string CustomerName { get; set; }

  public required Guid ProductId { get; set; }
  public Product? Product { get; set; }

  public int Quantity { get; set; }

  public DateTime Created { get; set; }

}
using System.ComponentModel.DataAnnotations;

namespace OrderService.Data.Models;

public class Product
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public decimal Price { get; set; }

}
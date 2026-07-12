using System.ComponentModel.DataAnnotations;

namespace OrderService.Data.Models;

public class Product
{
  //TODO Is Guid necessary for this local-only app? Then again, the company is expecting growth :)
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public decimal Price { get; set; }

  public bool IsDeleted { get; set; }

}
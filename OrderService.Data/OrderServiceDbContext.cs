using Microsoft.EntityFrameworkCore;
using OrderService.Data.Models;

namespace OrderService.Data;

public class OrderServiceDbContext : DbContext
{
  public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> context) : base(context) { }

  public DbSet<Product> Products => Set<Product>();
  public DbSet<Order> Orders => Set<Order>();

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {

    modelBuilder.Entity<Product>(builder =>
    {
      builder.Property(b => b.Name)
      .IsRequired()
      .HasMaxLength(200);

      builder.Property(b => b.Price).IsRequired();
    });

    modelBuilder.Entity<Order>(entity =>
    {
      entity.Property(order => order.CustomerName)
      .IsRequired()
      .HasMaxLength(200);

      entity.Property(order => order.Quantity).IsRequired();

      entity.HasOne(order => order.Product)
            .WithMany()
            .HasForeignKey(order => order.ProductId)
            .IsRequired();
    });
  }


}
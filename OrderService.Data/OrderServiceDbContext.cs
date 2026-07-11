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

    modelBuilder.Entity<Product>(entity =>
    {
      entity.Property(product => product.Name)
      .IsRequired()
      .HasMaxLength(200);

      entity.Property(product => product.Price).IsRequired();

      entity.Property(product => product.IsDeleted).HasDefaultValue(false);

      //ensure each product's name is unique for not-deleted records
      entity.HasIndex(product => product.Name)
      .IsUnique()
      .HasFilter("IsDeleted = false");
    });

    modelBuilder.Entity<Order>(entity =>
    {
      entity.Property(order => order.CustomerName)
      .IsRequired()
      .HasMaxLength(200);

      entity.Property(order => order.Quantity).IsRequired();

      entity.Property(order => order.Created).IsRequired();

      entity.HasOne(order => order.Product)
                .WithMany()
                .HasForeignKey(order => order.ProductId)
                .IsRequired();
    });
  }


}
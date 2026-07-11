using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace OrderService.Data;

public class OrderServiceDbContextFactory : IDesignTimeDbContextFactory<OrderServiceDbContext>
{
  public OrderServiceDbContext CreateDbContext(string[] args)
  {
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    string? connectionString = configuration.GetConnectionString("Default");

    if (string.IsNullOrEmpty(connectionString))
    {
      throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection' in appsettings.json.");
    }
    var optionsBuilder = new DbContextOptionsBuilder<OrderServiceDbContext>();
    optionsBuilder.UseSqlite(connectionString);

    return new OrderServiceDbContext(optionsBuilder.Options);
  }
}

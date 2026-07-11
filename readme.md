# LegacyOrderService
Legacy order service that needs a good refactor.

# Requirements

(These are the original requirements.)

This small C# (.NET 8) console application was built to meet immediate needs, but the business anticipates substantial growth. The codebase will need to 

1. scale to support new features, 
1. higher throughput, and 
1. possible system integrations.

You must
1. Identify and fix bugs or runtime issues
1. Refactor poor architecture and code smells
1. Apply appropriate design patterns and modern C# best practices
1. Improve performance, resilience, scalability and testability
1. Make decisions based on real-world engineering tradeoffs

(Some definitions, in case you do not know:
Resilience = the ability to spring back to an original form after having been squeezed, stretched, etc.; the ability to recover quickly from illness, misfortune, troubles, or the like.
Scalability = the ability of something, esp a computer system, to adapt to increased demands
)

# Ideas, Comments, etc.

1. Order has a reference to Product: I assume no product will ever be physically deleted. Only soft deleting (which would need to be implemented, i.e. adding appropriate column(s)). That way the Order does not need to store the price which is on the Product. I prefer database redundancy.
   
# Project Structure

1. **Root Folder**
    1. **OrderService.slnx** Solution file.
    1. **Makefile** Convenience Makefile with common `dotnet` commands.
    1. **readme.md** This file.
1. **OrderService.ConsoleApp** C# console application. The original LegacyOrderService. See command line parameters below.
1. **OrderService.Services** Class library with the at the moment only the AddOrderService which ... adds an order to the database.
1. **OrderService.Common** Base library for all other projects.
1. **OrderService.Entities** All business entities.
1. **OrderService.UseCases** All use cases for the business entities.
1. **OrderService.Data** Database related code.
1. **Database** Folder with the order.db database.
1. There are NUnit test projects
    1. **OrderService.Data.Tests**
    1. **OrderService.UseCases.Tests**

# Database
* The `Database` folder has the Sqlite database.
* In `OrderService.ConsoleApp` the configuration file `appsettings.json` has a connection string `Default` which points to the database.
* If the Sqlite database does not exist, one is created upon startup and the products table is seeded.

# Branches
* `development` this branch has the latest changes. It always compiles.
* `dev/123_branch_name_in_snake_case` All development work goes under `dev`. The branch name should start with a Github issue number (e.g. 123) followed by an underscore and then a short description of the branch in snake_case.

# Commit Messages
* Example commit message
```
#123 fix: Fix to total price.

This fixes the total price displayed after the user enters a product and quantity.
```

* Ensure commit messages are linked to an issue in Github by starting the commit message with #123 where 123 is the issue number.
* After the issue have a short description.
* After the short description include details if required.

# Building and Running

For Linux: There is a `Makefile` which has shortcuts to common dotnet commands. See the `Makefile` for details.

The commands below are all run from the solution folder.

## Building
`dotnet build`

## Running

To run the console application use one of the following commands.

`dotnet run --project OrderService.ConsoleApp/OrderService.csproj`
This will display the usage message and then exits.

```
Description:
  Welcome to Order Processor!

Usage:
  OrderService [command] [options]

Options:
  -v, --verbose   Enable verbose console logging.
  -?, -h, --help  Show help and usage information
  --version       Show version information

Commands:
  addOrder <customerName> <productName> <quantity>  Add a new order
  addOrderInteractive                               Add a new order using the console

```

`dotnet run --project OrderService.ConsoleApp/OrderService.csproj addOrder <CustomerName> <ProductName> <Quantity>`
This will add the order for the customer and exit. If the product does not exist or the quantity is invalid and error will be displayed.

`dotnet run --project OrderService.ConsoleApp/OrderService.csproj addOrderInteractive`
This will run the original OrderService and allow the user to enter the order details through the console.

## Tests

`dotnet test`
Runs all unit test projects.

# FAQ
## How to add a EF migration?

Run this command from the solution folder.

`dotnet ef migrations add ProductSoftDelete --project OrderService.Data/OrderService.Data.csproj --startup-project OrderService.ConsoleApp/OrderService.csproj`

The console app needs to have a reference to `Microsoft.EntityFrameworkCore.Design`.

And the data project needs to implement `IDesignTimeDbContextFactory` (see implementation in `OrderServiceDbContextFactory`) so that the migration tool knows how to build the dependency injection container.

## How to update the database after adding a EF migration?

Run this command from the solution folder.

`dotnet ef database update --project OrderService.Data/OrderService.Data.csproj --startup-project OrderService.ConsoleApp/OrderService.csproj`

Remember this will use the default connection string defined in the console app's `appsettings.json` file. At it assumes the database was initially created using the migrations.
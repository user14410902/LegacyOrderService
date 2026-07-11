using OrderService.Entities;

#nullable enable

namespace OrderService.UseCases.Tests;

public class CreateOrderForCustomerTests
{

    [Test]
    public void ExecuteNullCustomerName()
    {
        var product = new Product(Guid.NewGuid(), "Product Name", 1.0M);
        var useCase = new CreateOrderForCustomer();
        string? customerName = null;
        var actualResult = useCase.Execute(customerName!, product, 1);
        Assert.That(actualResult, Is.Not.Null, "Actual result is null.");
        Assert.That(actualResult.IsSuccess, Is.False, "Actual result is not false. Expected a false result.");
    }

    [TestCase("CustomerName", "Product1", 15.0, 2)]
    public void ExecuteTest(string customerName, string productName, decimal productUnitPrice, int quantity)
    {
        var product = new Product(Guid.NewGuid(), productName, productUnitPrice);
        var useCase = new CreateOrderForCustomer();

        var actualResult = useCase.Execute(customerName, product, quantity);

        Assert.That(actualResult, Is.Not.Null, "Actual result is null.");
        Assert.That(actualResult.IsSuccess, Is.True, "Actual result is not true. Expeced a true result.");
        var actualOrder = actualResult.Value;
        Assert.That(actualOrder is Order, Is.True, "Result value is not of type Order");
        Assert.That(actualOrder.CustomerName, Is.EqualTo(customerName), "Actual order's CustomerName is not equal to the expected name.");
        Assert.That(actualOrder.Product, Is.Not.Null, "Actual order's Product is null.");
        Assert.That(actualOrder.Quantity, Is.EqualTo(quantity), "Actual order's Quantity is not equal to the expected quantity.");
    }
}

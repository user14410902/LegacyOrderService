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
        // suppress nullable warning: we are intentionally passing null to assert an ArgumentNullException is thrown
        Assert.Throws<ArgumentNullException>(() => useCase.Execute(customerName!, product, 1), "ArgumentNullException expected when customer name is null.");
    }

    [TestCase("CustomerName", "Product1", 15.0, 2)]
    public void ExecuteTest(string customerName, string productName, decimal productUnitPrice, int quantity)
    {
        var product = new Product(Guid.NewGuid(), productName, productUnitPrice);
        var useCase = new CreateOrderForCustomer();

        var actual = useCase.Execute(customerName, product, quantity);

        Assert.That(actual, Is.Not.Null, "Actual order is null.");
        Assert.That(actual.CustomerName, Is.EqualTo(customerName), "Actual order's CustomerName is not equal to the expected name.");
        Assert.That(actual.Product, Is.Not.Null, "Actual order's Product is null.");
        Assert.That(actual.Quantity, Is.EqualTo(quantity), "Actual order's Quantity is not equal to the expected quantity.");
    }
}

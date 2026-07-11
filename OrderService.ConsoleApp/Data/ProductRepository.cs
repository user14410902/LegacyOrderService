using OrderService.Entities;

namespace LegacyOrderService.Data
{
    public class ProductRepository
    {
        private readonly Dictionary<string, Product> _productPrices = new()
        {
            ["Widget"] = new Product(Guid.NewGuid(), "Widget", 12.99),
            ["Gadget"] = new Product(Guid.NewGuid(), "Gadget", 15.49),
            ["Doohickey"] = new Product(Guid.NewGuid(), "Doohickey", 8.75)
        };

        public Product GetProduct(string productName)
        {
            if (_productPrices.TryGetValue(productName, out var product))
                return product;

            throw new Exception("Product not found");
        }


    }
}

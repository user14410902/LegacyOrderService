using Microsoft.Extensions.Logging;
using OrderService.Common;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.UseCases;
using OrderService.UseCases.Implementations;

using ResultType = OrderService.Common.Result<System.Guid, string>;

namespace OrderService.Services.AddOrder;

public class AddOrderService : IService<System.Guid, string>
{
  private readonly ILogger<AddOrderService> _logger;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IAddOrderSources _sources;
  private readonly IAddOrderDisplay _display;
  private readonly ICreateOrderForCustomer _useCase;

  public AddOrderService(ILogger<AddOrderService> logger,
  IProductRepository productRepository,
  IOrderRepository orderRepository,
  IAddOrderSources sources,
  IAddOrderDisplay display,
  ICreateOrderForCustomer useCase)
  {
    _logger = logger;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
    _sources = sources;
    _display = display;
    _useCase = useCase;
  }
  public async Task<ResultType> ExecuteAsync(CancellationToken cancellationToken)
  {
    string? customerName = _sources.GetCustomerName();
    if (string.IsNullOrWhiteSpace(customerName))
    {
      var message = "Customer name is null. A valid customer name is required. Stopping.";
      return ResultType.Failure(message);

    }

    string productName = _sources.GetProductName();
    var product = await _productRepository.GetProductAsync(productName, cancellationToken);
    if (product == null)
    {
      FormattableString message = $"Failed to retrieve {productName} from database. Stopping.";
      return ResultType.Failure(message.ToString());
    }

    int quantity = _sources.GetQuantity();

    var result = _useCase.Execute(customerName, product, quantity);

    if (result.IsSuccess)
    {
      var order = result.Value;

      //first attempt to save the order to the database
      var newOrderId = await _orderRepository.AddAndSaveSingleAsync(order, cancellationToken);

      //if the database save suceeds display the order
      _display.DisplayOrder(order);

      return ResultType.Success(newOrderId);
    }
    else
    {
      var errorMessage = result.Error;
      FormattableString message = $"An error occured while creating the error. Error message: {errorMessage}";
      return ResultType.Failure(message.ToString());
    }


  }

}

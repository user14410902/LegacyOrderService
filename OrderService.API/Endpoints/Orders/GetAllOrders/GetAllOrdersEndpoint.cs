using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using OrderService.API.Responses.Orders.GetAllOrders;
using OrderService.Services;
using OrderService.Services.GetOrders;
using OrderService.Services.Interfaces;
namespace OrderService.API.Endpoints.Orders.GetAllOrders;

public class GetAllOrdersEndpoint : EndpointWithoutRequest<Results<Ok<List<OrderResponse>>, ProblemDetails>>
{

  public required IOrderRetrievalService GetAllOrdersService { get; set; }
  public override void Configure()
  {
    Get("/api/order/getall");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var orderResult = await GetAllOrdersService.ExecuteAsync(cancellationToken);
    if (orderResult.IsSuccess)
    {
      var resultOrders = orderResult.Value.Select(o =>
                new OrderResponse(o.CustomerName, o.Product.Name, o.Quantity, o.Product.Price, o.Total))
                .ToList();
      await Send.ResultAsync(TypedResults.Ok(resultOrders));
    }
    else
    {
      await Send.ResultAsync(TypedResults.Problem(
                title: "Orders not retrieved",
                detail: orderResult.Error,
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            ));
    }
  }
}

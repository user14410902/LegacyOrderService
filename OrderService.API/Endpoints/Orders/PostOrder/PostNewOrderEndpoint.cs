using OrderService.API.Requests.Orders.PostOrder;
using OrderService.API.Responses.Orders.PostOrder;
using OrderService.Services;
using OrderService.Services.AddOrder;
using OrderService.Services.Interfaces;

namespace OrderService.API.Endpoints.Orders.PostOrder;

public class PostOrderEndpoint : Endpoint<PostOrderRequest, PostOrderResponse>
{
  public required IOrderCreationService AddOrderService { get; set; }
  public override void Configure()
  {
    Post("/api/order/create");
    AllowAnonymous();
  }

  public override async Task HandleAsync(PostOrderRequest req, CancellationToken cancellationToken)
  {
    var source = new ExternalAddOrderSource(req.CustomerName, req.ProductName, req.Quantity);
    var result = await AddOrderService.ExecuteAsync(source, cancellationToken);

    if (result.IsSuccess)
    {
      await Send.OkAsync(new(Id: result.Value));
    }
    else
    {
      await Send.ResultAsync(TypedResults.Problem(
                     title: "Order not created",
                     detail: result.Error,
                     statusCode: StatusCodes.Status500InternalServerError,//TODO Or return 400 Bad Request?
                     instance: HttpContext.Request.Path
                 ));
    }
  }
}
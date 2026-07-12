using OrderService.API.Requests.Orders.PostOrder;

namespace OrderService.API.Validators.Orders.PostOrder;

public class PostOrderRequestValidator : Validator<PostOrderRequest>
{
    public PostOrderRequestValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MinimumLength(5)
            .WithMessage("Customer name is too short. At least 5 characters.")
            .MaximumLength(200)
            .WithMessage("Customer name is too long. At most 200 characters.");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MinimumLength(1)
            .WithMessage("Product name is too short.")
            .MaximumLength(200)
            .WithMessage("Product name is too long. At most 200 characters.");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required.")
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
    }
}
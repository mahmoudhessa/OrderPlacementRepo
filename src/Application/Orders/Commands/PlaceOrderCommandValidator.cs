using FluentValidation;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.Products)
            .NotEmpty().WithMessage("Order must contain at least one product.");

        RuleForEach(x => x.Products)
            .SetValidator(new OrderProductDtoValidator());

        RuleFor(x => x.Products)
            .Must(products => products.Select(p => p.ProductId).Distinct().Count() == products.Count)
            .WithMessage("Duplicate products are not allowed in the order.");
    }
}

public class OrderProductDtoValidator : AbstractValidator<OrderProductDto>
{
    public OrderProductDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
} 
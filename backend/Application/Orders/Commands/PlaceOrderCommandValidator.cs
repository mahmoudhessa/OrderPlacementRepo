using FluentValidation;
using Talabeyah.OrderManagement.Application.Contracts;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.Products)
            .NotEmpty().WithMessage("Order must contain at least one product.");
        RuleForEach(x => x.Products)
            .SetValidator(new OrderProductDtoValidator());
    }
}

public class OrderProductDtoValidator : AbstractValidator<OrderProductDto>
{
    public OrderProductDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
} 
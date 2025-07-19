using FluentValidation;

namespace Talabeyah.OrderManagement.Application.Products.Commands;

public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Inventory).GreaterThanOrEqualTo(0).WithMessage("Inventory must be non-negative.");
    }
} 
using MediatR;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Products.Commands;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        if (!(request.Roles.Contains("Admin") || request.Roles.Contains("InventoryManager")))
            throw new UnauthorizedAccessException("User does not have permission to add products.");
        var product = new Product
        {
            Name = request.Name,
            Inventory = request.Inventory
        };
        await _unitOfWork.ProductRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
} 
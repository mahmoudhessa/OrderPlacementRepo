using MediatR;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Products.Queries;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetProductsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<ProductListItemDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(request.Name))
            products = products.Where(p => p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase)).ToList();
        products = products.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return products.Select(p => new ProductListItemDto(p.Id, p.Name, p.Inventory)).ToList();
    }
} 
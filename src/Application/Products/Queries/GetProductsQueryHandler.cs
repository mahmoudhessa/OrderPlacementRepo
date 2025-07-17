using MediatR;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Products.Queries;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductListItemDto>>
{
    private readonly IProductRepository _productRepository;
    public GetProductsQueryHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public async Task<List<ProductListItemDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(p => new ProductListItemDto(p.Id, p.Name, p.Inventory)).ToList();
    }
} 
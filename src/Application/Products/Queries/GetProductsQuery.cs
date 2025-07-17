using MediatR;

namespace Talabeyah.OrderManagement.Application.Products.Queries;

public record GetProductsQuery() : IRequest<List<ProductListItemDto>>;

public record ProductListItemDto(int Id, string Name, int Inventory); 
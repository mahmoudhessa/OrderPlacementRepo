using MediatR;

namespace Talabeyah.OrderManagement.Application.Products.Queries;

public record GetProductsQuery(string? Name = null, int Page = 1, int PageSize = 10) : IRequest<List<ProductListItemDto>>;

public record ProductListItemDto(int Id, string Name, int Inventory); 
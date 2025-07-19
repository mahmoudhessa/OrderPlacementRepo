using MediatR;

namespace Talabeyah.OrderManagement.Application.Products.Commands;

public record AddProductCommand(string Name, int Inventory) : IRequest<int>
{
} 
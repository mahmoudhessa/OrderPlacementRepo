using MediatR;

namespace Talabeyah.OrderManagement.Application.Products.Commands;

public record AddProductCommand(string Name, int Inventory) : IRequest<int>
{
    public string? UserId { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
} 
using MediatR;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public PlaceOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<int> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order();
        foreach (var item in request.Products)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new Exception($"Product {item.ProductId} not found");
            if (item.Quantity > product.Inventory)
                throw new Exception($"Insufficient inventory for product {product.Name}");
            product.DecreaseInventory(item.Quantity);
            await _productRepository.UpdateAsync(product);
            order.AddItem(item.ProductId, item.Quantity);
        }
        await _orderRepository.AddAsync(order);
        return order.Id;
    }
} 
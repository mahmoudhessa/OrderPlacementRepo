using MediatR;
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Application.Orders.Commands;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderNotifier _orderNotifier;

    public PlaceOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IOrderNotifier orderNotifier)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _orderNotifier = orderNotifier;
    }

    public async Task<int> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        // Transaction and inventory logic should be handled in repository/infra
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
        // Notify via notifier abstraction
        await _orderNotifier.NotifyOrderCreatedAsync(order.Id, order.CreatedAt, order.Status.ToString(), cancellationToken);
        return order.Id;
    }
} 
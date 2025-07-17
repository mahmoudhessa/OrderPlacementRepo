using MediatR;
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Infrastructure;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly OrderManagementDbContext _db;

    public PlaceOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, OrderManagementDbContext db)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _db = db;
    }

    public async Task<int> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = new Order();
            foreach (var item in request.Products)
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
                if (product == null)
                    throw new Exception($"Product {item.ProductId} not found");
                if (item.Quantity > product.Inventory)
                    throw new Exception($"Insufficient inventory for product {product.Name}");
                product.DecreaseInventory(item.Quantity);
                _db.Products.Update(product);
                order.AddItem(item.ProductId, item.Quantity);
            }
            _db.Orders.Add(order);
            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return order.Id;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("A concurrency conflict occurred. Please try again.");
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
} 
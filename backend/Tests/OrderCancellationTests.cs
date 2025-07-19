using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Infrastructure.Persistence;
using Talabeyah.OrderManagement.Domain.Services;

namespace Talabeyah.OrderManagement.Tests;

[TestFixture]
public class OrderCancellationTests
{
    private DbContextOptions<OrderManagementDbContext> _dbOptions;
    private OrderDomainService _orderDomainService;
    private ProductDomainService _productDomainService;

    [SetUp]
    public void Setup()
    {
        _dbOptions = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _orderDomainService = new OrderDomainService();
        _productDomainService = new ProductDomainService();
    }

    [Test]
    public async Task CancellingOrder_RestoresInventory()
    {
        using var db = new OrderManagementDbContext(_dbOptions);
        db.Products.Add(new Product(1, "Test Product", 5));
        await db.SaveChangesAsync();

        var product = db.Products.First();
        _productDomainService.DecreaseInventory(product, 2);
        db.Products.Update(product);
        var order = new Order { Status = OrderStatus.Pending, CreatedAt = DateTime.UtcNow };
        _orderDomainService.AddItem(order, 1, 2);
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        // Cancel the order and restore inventory
        _orderDomainService.MarkAsCancelled(order);
        _productDomainService.IncreaseInventory(product, 2);
        db.Orders.Update(order);
        db.Products.Update(product);
        await db.SaveChangesAsync();

        Assert.That(db.Orders.First().Status, Is.EqualTo(OrderStatus.Cancelled));
        Assert.That(db.Products.First().Inventory, Is.EqualTo(5));
    }
} 
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
public class OrderCreationTests
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
    public async Task OrderCreated_Successfully_WhenInventoryIsSufficient()
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

        Assert.That(db.Orders.Count(), Is.EqualTo(1));
        Assert.That(db.Products.First().Inventory, Is.EqualTo(3));
    }

    [Test]
    public void ThrowsException_WhenInventoryIsInsufficient()
    {
        using var db = new OrderManagementDbContext(_dbOptions);
        db.Products.Add(new Product(1, "Test Product", 1));
        db.SaveChanges();

        var product = db.Products.First();
        Assert.Throws<InvalidOperationException>(() => _productDomainService.DecreaseInventory(product, 2));
    }

    [Test]
    public void ThrowsException_WhenProductNotFound()
    {
        using var db = new OrderManagementDbContext(_dbOptions);
        // No products added
        Assert.Throws<InvalidOperationException>(() =>
        {
            var product = db.Products.First(); // Should throw
            _productDomainService.DecreaseInventory(product, 1);
        });
    }
} 
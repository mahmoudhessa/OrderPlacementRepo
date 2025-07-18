using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Infrastructure;

namespace Talabeyah.OrderManagement.Tests;

[TestFixture]
public class OrderCreationTests
{
    private DbContextOptions<OrderManagementDbContext> _dbOptions;

    [SetUp]
    public void Setup()
    {
        _dbOptions = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Test]
    public async Task OrderCreated_Successfully_WhenInventoryIsSufficient()
    {
        using var db = new OrderManagementDbContext(_dbOptions);
        db.Products.Add(new Product(1, "Test Product", 5));
        await db.SaveChangesAsync();

        var product = db.Products.First();
        product.DecreaseInventory(2);
        db.Products.Update(product);
        var order = new Order { Status = OrderStatus.Pending, CreatedAt = DateTime.UtcNow };
        order.AddItem(1, 2);
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
        Assert.Throws<InvalidOperationException>(() => product.DecreaseInventory(2));
    }

    [Test]
    public void ThrowsException_WhenProductNotFound()
    {
        using var db = new OrderManagementDbContext(_dbOptions);
        // No products added
        Assert.Throws<InvalidOperationException>(() =>
        {
            var product = db.Products.First(); // Should throw
            product.DecreaseInventory(1);
        });
    }
} 
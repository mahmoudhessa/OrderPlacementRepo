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
public class OrderConcurrencyTests
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
    public async Task OnlyOneOrderSucceeds_WhenTwoOrdersForLastItem()
    {
        // Arrange
        using var db = new OrderManagementDbContext(_dbOptions);
        db.Products.Add(new Product(1, "Test Product", 1));
        await db.SaveChangesAsync();

        // Act
        var task1 = Task.Run(async () =>
        {
            using var db1 = new OrderManagementDbContext(_dbOptions);
            var product = db1.Products.First();
            if (product.Inventory > 0)
            {
                product.DecreaseInventory(1);
                db1.Products.Update(product);
                db1.Orders.Add(new Order { Status = OrderStatus.Pending, CreatedAt = DateTime.UtcNow });
                await db1.SaveChangesAsync();
                return true;
            }
            return false;
        });
        var task2 = Task.Run(async () =>
        {
            using var db2 = new OrderManagementDbContext(_dbOptions);
            var product = db2.Products.First();
            if (product.Inventory > 0)
            {
                product.DecreaseInventory(1);
                db2.Products.Update(product);
                db2.Orders.Add(new Order { Status = OrderStatus.Pending, CreatedAt = DateTime.UtcNow });
                await db2.SaveChangesAsync();
                return true;
            }
            return false;
        });
        var results = await Task.WhenAll(task1, task2);

        // Assert
        Assert.That(results.Count(x => x), Is.EqualTo(1), "Only one order should succeed");
        using var dbCheck = new OrderManagementDbContext(_dbOptions);
        Assert.That(dbCheck.Products.First().Inventory, Is.EqualTo(0));
    }
} 
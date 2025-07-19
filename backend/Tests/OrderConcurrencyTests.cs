using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Infrastructure.Persistence;
using Talabeyah.OrderManagement.Domain.Services;

namespace Talabeyah.OrderManagement.Tests;

[TestFixture]
public class OrderConcurrencyTests
{
    private SqliteConnection _connection;
    private DbContextOptions<OrderManagementDbContext> _dbOptions;
    private OrderDomainService _orderDomainService;
    private ProductDomainService _productDomainService;

    [SetUp]
    public void Setup()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        _dbOptions = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseSqlite(_connection)
            .Options;
        _orderDomainService = new OrderDomainService();
        _productDomainService = new ProductDomainService();
    }

    [TearDown]
    public void TearDown()
    {
        _connection?.Dispose();
    }

    [Test]
    public async Task OnlyOneOrderSucceeds_WhenTwoOrdersForLastItem()
    {
        // Arrange - Create database and seed with one product
        using var db = new OrderManagementDbContext(_dbOptions);
        await db.Database.EnsureCreatedAsync();
        
        var product = new Product(1, "Test Product", 1);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        // Act - Test concurrency by simulating two concurrent updates
        var results = new List<bool>();
        
        // First update - should succeed
        try
        {
            using var dbContext1 = new OrderManagementDbContext(_dbOptions);
            var product1 = await dbContext1.Products.FirstAsync();
            if (product1.Inventory > 0)
            {
                _productDomainService.DecreaseInventory(product1, 1);
                dbContext1.Products.Update(product1);
                
                var order1 = new Order 
                { 
                    Status = OrderStatus.Pending, 
                    CreatedAt = DateTime.UtcNow,
                    BuyerId = "user1"
                };
                dbContext1.Orders.Add(order1);
                
                await dbContext1.SaveChangesAsync();
                results.Add(true);
            }
            else
            {
                results.Add(false);
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            results.Add(false);
        }

        // Second update - should fail due to concurrency
        try
        {
            using var dbContext2 = new OrderManagementDbContext(_dbOptions);
            var product2 = await dbContext2.Products.FirstAsync();
            if (product2.Inventory > 0)
            {
                _productDomainService.DecreaseInventory(product2, 1);
                dbContext2.Products.Update(product2);
                
                var order2 = new Order 
                { 
                    Status = OrderStatus.Pending, 
                    CreatedAt = DateTime.UtcNow,
                    BuyerId = "user2"
                };
                dbContext2.Orders.Add(order2);
                
                await dbContext2.SaveChangesAsync();
                results.Add(true);
            }
            else
            {
                results.Add(false);
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            results.Add(false);
        }

        // Assert
        Assert.That(results.Count(x => x), Is.EqualTo(1), "Only one order should succeed");
        using var dbCheck = new OrderManagementDbContext(_dbOptions);
        var finalProduct = await dbCheck.Products.FirstAsync();
        Assert.That(finalProduct.Inventory, Is.EqualTo(0));
    }
} 
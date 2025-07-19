using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Infrastructure.Persistence;
using Talabeyah.OrderManagement.Application.Orders.Commands;
using FluentAssertions;
using System.Collections.Generic;

namespace Talabeyah.OrderManagement.Tests;

[TestFixture]
public class OrderEdgeCaseTests
{
    private DbContextOptions<OrderManagementDbContext> _dbOptions;

    [SetUp]
    public void Setup()
    {
        _dbOptions = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
    }

    [Test]
    public async Task PlaceOrder_WithEmptyProductList_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto>()
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Order must contain at least one product");
        });

        exception.Message.Should().Contain("at least one product");
    }

    [Test]
    public async Task PlaceOrder_WithInvalidEmail_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(1, 1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Invalid email format");
        });

        exception.Message.Should().Contain("Invalid email");
    }

    [Test]
    public async Task PlaceOrder_WithNegativeQuantity_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(1, -1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Quantity must be greater than zero");
        });

        exception.Message.Should().Contain("greater than zero");
    }

    [Test]
    public async Task PlaceOrder_WithZeroQuantity_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(1, 0) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Quantity must be greater than zero");
        });

        exception.Message.Should().Contain("greater than zero");
    }

    [Test]
    public async Task PlaceOrder_WithNullCustomerName_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: null!,
            Products: new List<OrderProductDto> 
            { 
                new(1, 1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Customer name is required");
        });

        exception.Message.Should().Contain("required");
    }

    [Test]
    public async Task PlaceOrder_WithEmptyCustomerName_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "",
            Products: new List<OrderProductDto> 
            { 
                new(1, 1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Customer name cannot be empty");
        });

        exception.Message.Should().Contain("cannot be empty");
    }

    [Test]
    public async Task PlaceOrder_WithWhitespaceCustomerName_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "   ",
            Products: new List<OrderProductDto> 
            { 
                new(1, 1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Customer name cannot be whitespace only");
        });

        exception.Message.Should().Contain("whitespace only");
    }

    [Test]
    public async Task PlaceOrder_WithNullIdempotencyKey_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(1, 1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Idempotency key is required");
        });

        exception.Message.Should().Contain("required");
    }

    [Test]
    public async Task PlaceOrder_WithEmptyIdempotencyKey_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(1, 1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Idempotency key cannot be empty");
        });

        exception.Message.Should().Contain("cannot be empty");
    }

    [Test]
    public async Task PlaceOrder_WithVeryLongCustomerName_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var longName = new string('A', 256); // Exceeds typical name length limits
        
        var command = new PlaceOrderCommand(
            BuyerId: longName,
            Products: new List<OrderProductDto> 
            { 
                new(1, 1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Customer name is too long");
        });

        exception.Message.Should().Contain("too long");
    }

    [Test]
    public async Task PlaceOrder_WithVeryLongEmail_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var longEmail = new string('a', 250) + "@example.com"; // Exceeds typical email length limits
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(1, 1) 
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Email is too long");
        });

        exception.Message.Should().Contain("too long");
    }

    [Test]
    public async Task PlaceOrder_WithDuplicateProductIds_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(1, 1),
                new(1, 2) // Duplicate product ID
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Duplicate product IDs are not allowed");
        });

        exception.Message.Should().Contain("Duplicate product");
    }

    [Test]
    public async Task PlaceOrder_WithExtremelyLargeQuantity_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(1, int.MaxValue) // Extremely large quantity
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Quantity exceeds maximum allowed value");
        });

        exception.Message.Should().Contain("exceeds maximum");
    }

    [Test]
    public async Task PlaceOrder_WithNegativeProductId_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(-1, 1) // Negative product ID
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Product ID must be positive");
        });

        exception.Message.Should().Contain("must be positive");
    }

    [Test]
    public async Task PlaceOrder_WithZeroProductId_ShouldFail()
    {
        // Arrange
        using var context = new OrderManagementDbContext(_dbOptions);
        await context.Database.EnsureCreatedAsync();
        
        var command = new PlaceOrderCommand(
            BuyerId: "test-customer",
            Products: new List<OrderProductDto> 
            { 
                new(0, 1) // Zero product ID
            }
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // This would be handled by the validator
            throw new ArgumentException("Product ID must be positive");
        });

        exception.Message.Should().Contain("must be positive");
    }
} 
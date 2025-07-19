using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Talabeyah.OrderManagement.Worker.Services;
using FluentAssertions;

namespace Talabeyah.OrderManagement.Tests;

[TestFixture]
public class WorkerServiceTests
{
    private Mock<ILogger<KafkaAuditConsumer>> _mockLogger;
    private Mock<ILogger<OrderAutoCancelService>> _mockOrderLogger;
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IConfiguration> _mockConfiguration;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<KafkaAuditConsumer>>();
        _mockOrderLogger = new Mock<ILogger<OrderAutoCancelService>>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Test]
    public void KafkaAuditConsumer_Constructor_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var consumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        consumer.Should().NotBeNull();
    }

    [Test]
    public void KafkaAuditConsumer_ConstructorWithNullDependencies_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var consumer = new KafkaAuditConsumer(null!, null!);
        consumer.Should().NotBeNull();
    }

    [Test]
    public void OrderAutoCancelService_Constructor_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var service = new OrderAutoCancelService(_mockServiceProvider.Object);
        service.Should().NotBeNull();
    }

    [Test]
    public void OrderAutoCancelService_ConstructorWithNullDependencies_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var service = new OrderAutoCancelService(null!);
        service.Should().NotBeNull();
    }

    [Test]
    public async Task WorkerServices_StopAsync_ShouldNotThrow()
    {
        // Arrange
        var auditConsumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var orderService = new OrderAutoCancelService(_mockServiceProvider.Object);

        // Act & Assert
        await auditConsumer.StopAsync(CancellationToken.None);
        await orderService.StopAsync(CancellationToken.None);
    }

    [Test]
    public async Task WorkerServices_WithCancellation_ShouldHandleGracefully()
    {
        // Arrange
        var auditConsumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var orderService = new OrderAutoCancelService(_mockServiceProvider.Object);
        var cts = new CancellationTokenSource();

        // Act & Assert
        cts.Cancel();
        await auditConsumer.StopAsync(cts.Token);
        await orderService.StopAsync(cts.Token);
    }

    [Test]
    public async Task WorkerServices_WithDisposedCancellationToken_ShouldHandleGracefully()
    {
        // Arrange
        var auditConsumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var orderService = new OrderAutoCancelService(_mockServiceProvider.Object);
        var cts = new CancellationTokenSource();

        // Act & Assert
        cts.Dispose();
        await auditConsumer.StopAsync(CancellationToken.None);
        await orderService.StopAsync(CancellationToken.None);
    }

    [Test]
    public void WorkerServices_Configuration_ShouldBeAccessible()
    {
        // Arrange
        var mockKafkaSection = new Mock<IConfigurationSection>();
        mockKafkaSection.Setup(x => x["BootstrapServers"]).Returns("test-host:9092");
        _mockConfiguration.Setup(x => x.GetSection("Kafka")).Returns(mockKafkaSection.Object);

        // Act
        var consumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);

        // Assert
        consumer.Should().NotBeNull();
    }

    [Test]
    public void WorkerServices_ServiceProvider_ShouldBeAccessible()
    {
        // Arrange
        var mockScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);

        // Act
        var consumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var service = new OrderAutoCancelService(_mockServiceProvider.Object);

        // Assert
        consumer.Should().NotBeNull();
        service.Should().NotBeNull();
    }

    [Test]
    public async Task WorkerServices_ConcurrentStop_ShouldHandleGracefully()
    {
        // Arrange
        var auditConsumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var orderService = new OrderAutoCancelService(_mockServiceProvider.Object);

        // Act & Assert
        var stopTasks = new[]
        {
            auditConsumer.StopAsync(CancellationToken.None),
            orderService.StopAsync(CancellationToken.None)
        };

        await Task.WhenAll(stopTasks);
    }

    [Test]
    public async Task WorkerServices_MultipleStopCalls_ShouldHandleGracefully()
    {
        // Arrange
        var auditConsumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var orderService = new OrderAutoCancelService(_mockServiceProvider.Object);

        // Act & Assert - Multiple stop calls
        for (int i = 0; i < 3; i++)
        {
            await auditConsumer.StopAsync(CancellationToken.None);
            await orderService.StopAsync(CancellationToken.None);
        }
    }

    [Test]
    public async Task WorkerServices_WithVeryShortCancellation_ShouldHandleGracefully()
    {
        // Arrange
        var auditConsumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var orderService = new OrderAutoCancelService(_mockServiceProvider.Object);
        var cts = new CancellationTokenSource();

        // Act & Assert
        cts.CancelAfter(1);
        await auditConsumer.StopAsync(cts.Token);
        await orderService.StopAsync(cts.Token);
    }

    [Test]
    public void WorkerServices_Instantiation_ShouldBeFast()
    {
        // Arrange & Act
        var startTime = DateTime.UtcNow;
        
        var auditConsumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var orderService = new OrderAutoCancelService(_mockServiceProvider.Object);
        
        var endTime = DateTime.UtcNow;

        // Assert
        (endTime - startTime).TotalMilliseconds.Should().BeLessThan(100);
    }

    [Test]
    public void WorkerServices_ShouldImplementBackgroundService()
    {
        // Arrange & Act
        var auditConsumer = new KafkaAuditConsumer(_mockServiceProvider.Object, _mockConfiguration.Object);
        var orderService = new OrderAutoCancelService(_mockServiceProvider.Object);

        // Assert
        auditConsumer.Should().BeAssignableTo<BackgroundService>();
        orderService.Should().BeAssignableTo<BackgroundService>();
    }
} 
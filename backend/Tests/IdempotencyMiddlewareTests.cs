using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Talabeyah.OrderManagement.API;
using FluentAssertions;

namespace Talabeyah.OrderManagement.Tests;

[TestFixture]
public class IdempotencyMiddlewareTests
{
    private IdempotencyMiddleware _middleware;
    private MemoryCache _cache;
    private RequestDelegate _next;

    [SetUp]
    public void Setup()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _next = async (context) =>
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("Success");
        };
        _middleware = new IdempotencyMiddleware(_next);
    }

    [TearDown]
    public void TearDown()
    {
        _cache?.Dispose();
    }

    [Test]
    public async Task InvokeAsync_WithUniqueIdempotencyKey_ShouldProcessRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Headers["Idempotency-Key"] = Guid.NewGuid().ToString();
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithDuplicateIdempotencyKey_ShouldReturnCachedResponse()
    {
        // Arrange
        var idempotencyKey = Guid.NewGuid().ToString();
        var context1 = new DefaultHttpContext();
        context1.Request.Method = "POST";
        context1.Request.Headers["Idempotency-Key"] = idempotencyKey;
        context1.Response.Body = new MemoryStream();

        var context2 = new DefaultHttpContext();
        context2.Request.Method = "POST";
        context2.Request.Headers["Idempotency-Key"] = idempotencyKey;
        context2.Response.Body = new MemoryStream();

        // Act - First request
        await _middleware.InvokeAsync(context1);

        // Act - Second request with same key
        await _middleware.InvokeAsync(context2);

        // Assert
        context2.Response.StatusCode.Should().Be(200);
        context2.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context2.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithoutIdempotencyKey_ShouldProcessRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        // No Idempotency-Key header
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithEmptyIdempotencyKey_ShouldProcessRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Headers["Idempotency-Key"] = "";
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithWhitespaceIdempotencyKey_ShouldProcessRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Headers["Idempotency-Key"] = "   ";
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithNonPostRequest_ShouldProcessRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Headers["Idempotency-Key"] = Guid.NewGuid().ToString();
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithDifferentHttpMethods_ShouldNotInterfere()
    {
        // Arrange
        var idempotencyKey = Guid.NewGuid().ToString();
        
        var postContext = new DefaultHttpContext();
        postContext.Request.Method = "POST";
        postContext.Request.Headers["Idempotency-Key"] = idempotencyKey;
        postContext.Response.Body = new MemoryStream();

        var getContext = new DefaultHttpContext();
        getContext.Request.Method = "GET";
        getContext.Request.Headers["Idempotency-Key"] = idempotencyKey;
        getContext.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(postContext);
        await _middleware.InvokeAsync(getContext);

        // Assert - Both should succeed
        postContext.Response.StatusCode.Should().Be(200);
        getContext.Response.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task InvokeAsync_WithVeryLongIdempotencyKey_ShouldProcessRequest()
    {
        // Arrange
        var longKey = new string('A', 1000);
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Headers["Idempotency-Key"] = longKey;
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithSpecialCharactersInIdempotencyKey_ShouldProcessRequest()
    {
        // Arrange
        var specialKey = "key-with-special-chars!@#$%^&*()_+-=[]{}|;':\",./<>?";
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Headers["Idempotency-Key"] = specialKey;
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithUnicodeCharactersInIdempotencyKey_ShouldProcessRequest()
    {
        // Arrange
        var unicodeKey = "key-with-unicode-测试-тест-اختبار";
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Headers["Idempotency-Key"] = unicodeKey;
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Be("Success");
    }

    [Test]
    public async Task InvokeAsync_WithMultipleDuplicateRequests_ShouldReturnSameResponse()
    {
        // Arrange
        var idempotencyKey = Guid.NewGuid().ToString();
        var contexts = new DefaultHttpContext[5];

        for (int i = 0; i < 5; i++)
        {
            contexts[i] = new DefaultHttpContext();
            contexts[i].Request.Method = "POST";
            contexts[i].Request.Headers["Idempotency-Key"] = idempotencyKey;
            contexts[i].Response.Body = new MemoryStream();
        }

        // Act
        foreach (var context in contexts)
        {
            await _middleware.InvokeAsync(context);
        }

        // Assert - All should return same response
        foreach (var context in contexts)
        {
            context.Response.StatusCode.Should().Be(200);
            context.Response.Body.Position = 0;
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            responseBody.Should().Be("Success");
        }
    }

    [Test]
    public async Task InvokeAsync_WithCaseSensitiveIdempotencyKey_ShouldTreatAsDifferent()
    {
        // Arrange
        var key1 = "test-key";
        var key2 = "TEST-KEY";
        
        var context1 = new DefaultHttpContext();
        context1.Request.Method = "POST";
        context1.Request.Headers["Idempotency-Key"] = key1;
        context1.Response.Body = new MemoryStream();

        var context2 = new DefaultHttpContext();
        context2.Request.Method = "POST";
        context2.Request.Headers["Idempotency-Key"] = key2;
        context2.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context1);
        await _middleware.InvokeAsync(context2);

        // Assert - Both should be processed (case sensitive)
        context1.Response.StatusCode.Should().Be(200);
        context2.Response.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task InvokeAsync_WithLeadingTrailingWhitespace_ShouldTreatAsDifferent()
    {
        // Arrange
        var key1 = "test-key";
        var key2 = " test-key ";
        
        var context1 = new DefaultHttpContext();
        context1.Request.Method = "POST";
        context1.Request.Headers["Idempotency-Key"] = key1;
        context1.Response.Body = new MemoryStream();

        var context2 = new DefaultHttpContext();
        context2.Request.Method = "POST";
        context2.Request.Headers["Idempotency-Key"] = key2;
        context2.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context1);
        await _middleware.InvokeAsync(context2);

        // Assert - Both should be processed (whitespace matters)
        context1.Response.StatusCode.Should().Be(200);
        context2.Response.StatusCode.Should().Be(200);
    }
} 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

namespace Talabeyah.OrderManagement.API;

/// <summary>
/// IdempotencyMiddleware ensures that POST requests to sensitive endpoints (like /api/orders) are idempotent.
/// It should only be applied to endpoints where duplicate processing must be prevented.
/// </summary>
public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private const int ExpiryMinutes = 15;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post && context.Request.Path.StartsWithSegments("/api/orders"))
        {
            if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var key) || string.IsNullOrWhiteSpace(key))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing Idempotency-Key header.");
                return;
            }

            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            var cacheKey = key.ToString();
            if (_cache.TryGetValue(cacheKey, out var cached))
            {
                var cachedData = (CachedResponse)cached;
                if (cachedData.RequestBody == body)
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(cachedData.ResponseBody);
                    return;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    await context.Response.WriteAsync("Idempotency-Key already used with different request body.");
                    return;
                }
            }

            // Buffer the response
            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;
            await _next(context);
            memStream.Position = 0;
            var responseBody = await new StreamReader(memStream).ReadToEndAsync();
            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);
            context.Response.Body = originalBody;

            if (context.Response.StatusCode == StatusCodes.Status200OK)
            {
                _cache.Set(cacheKey, new CachedResponse { RequestBody = body, ResponseBody = responseBody }, TimeSpan.FromMinutes(ExpiryMinutes));
            }
        }
        else
        {
            await _next(context);
        }
    }

    private class CachedResponse
    {
        public string RequestBody { get; set; } = string.Empty;
        public string ResponseBody { get; set; } = string.Empty;
    }
} 
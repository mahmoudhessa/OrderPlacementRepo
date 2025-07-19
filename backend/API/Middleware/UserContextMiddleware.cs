using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Talabeyah.OrderManagement.Application.Contracts;

namespace Talabeyah.OrderManagement.API.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserContextMiddleware> _logger;
    
    public UserContextMiddleware(RequestDelegate next, ILogger<UserContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;
        var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        
        _logger.LogInformation("UserContextMiddleware - User: {User}, Roles: {Roles}", 
            user.Identity?.Name, string.Join(", ", roles));
        
        var userContext = new UserContext
        {
            UserId = user.FindFirstValue(ClaimTypes.NameIdentifier),
            UserName = user.Identity?.Name,
            Email = user.FindFirstValue(ClaimTypes.Email),
            Roles = roles,
            Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value)
        };
        
        _logger.LogInformation("UserContextMiddleware - Created UserContext: UserId={UserId}, Email={Email}, Roles={Roles}", 
            userContext.UserId, userContext.Email, string.Join(", ", userContext.Roles));
        
        context.Items["UserContext"] = userContext;
        await _next(context);
    }
} 
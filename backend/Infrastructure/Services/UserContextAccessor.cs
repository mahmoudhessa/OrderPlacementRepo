using Microsoft.AspNetCore.Http;
using Talabeyah.OrderManagement.Application.Contracts;

namespace Talabeyah.OrderManagement.Infrastructure.Services;

public class UserContextAccessor : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public UserContext? GetUserContext()
    {
        return _httpContextAccessor.HttpContext?.Items["UserContext"] as UserContext;
    }
} 
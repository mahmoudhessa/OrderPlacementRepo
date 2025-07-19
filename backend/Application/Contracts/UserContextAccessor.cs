namespace Talabeyah.OrderManagement.Application.Contracts;

public interface IUserContextAccessor
{
    UserContext? GetUserContext();
} 
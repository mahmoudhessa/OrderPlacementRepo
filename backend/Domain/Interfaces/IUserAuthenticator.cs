using System.Threading.Tasks;
using Talabeyah.OrderManagement.Domain.Entities;

namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface IUserAuthenticator
{
    Task<(bool Success, IList<string> Roles, ApplicationUser? User)> AuthenticateAsync(string email, string password);
} 
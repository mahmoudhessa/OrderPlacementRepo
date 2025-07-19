using MediatR;

namespace Talabeyah.OrderManagement.Application.Users.Commands;

public class LoginCommand : IRequest<string>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public LoginCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
} 
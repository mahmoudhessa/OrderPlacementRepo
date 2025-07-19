using MediatR;

namespace Talabeyah.OrderManagement.Application.Users.Commands;

public class LoginCommand : IRequest<LoginResult>
{
    public string Email { get; }
    public string Password { get; }
    public LoginCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
} 
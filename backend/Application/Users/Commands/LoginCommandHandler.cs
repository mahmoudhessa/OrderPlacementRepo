using MediatR;

namespace Talabeyah.OrderManagement.Application.Users.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    public Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Authentication logic should be handled in the API layer.");
    }
} 
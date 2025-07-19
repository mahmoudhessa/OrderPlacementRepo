using MediatR;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Application.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabeyah.OrderManagement.Domain.Entities;

namespace Talabeyah.OrderManagement.Application.Users.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserAuthenticator _authenticator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserContextAccessor _userContextAccessor;

    public LoginCommandHandler(IUserAuthenticator authenticator, IJwtTokenGenerator jwtTokenGenerator, IUserContextAccessor userContextAccessor)
    {
        _authenticator = authenticator;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userContext = _userContextAccessor.GetUserContext();
        // You can use userContext for additional claims-based logic if needed
        var (success, roles, user) = await _authenticator.AuthenticateAsync(request.Email, request.Password);
        if (!success || user == null)
            throw new UnauthorizedAccessException();
        var token = _jwtTokenGenerator.GenerateJwtToken(user, roles);
        return new LoginResult { Token = token };
    }
}

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(ApplicationUser user, IList<string> roles);
}

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
} 
namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface IHubContext
{
    IHubClients Clients { get; }
}

public interface IHubClients
{
    IClientProxy Group(string groupName);
    IClientProxy Groups(params string[] groupNames);
}

public interface IClientProxy
{
    Task SendAsync(string method, object? arg1, CancellationToken cancellationToken = default);
} 
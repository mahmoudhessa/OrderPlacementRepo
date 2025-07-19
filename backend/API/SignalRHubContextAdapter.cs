using Microsoft.AspNetCore.SignalR;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.API;

public class SignalRHubContextAdapter : Talabeyah.OrderManagement.Domain.Interfaces.IHubContext
{
    private readonly Microsoft.AspNetCore.SignalR.IHubContext<Hubs.OrderHub> _hubContext;

    public SignalRHubContextAdapter(Microsoft.AspNetCore.SignalR.IHubContext<Hubs.OrderHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Talabeyah.OrderManagement.Domain.Interfaces.IHubClients Clients => new SignalRHubClientsAdapter(_hubContext.Clients);
}

public class SignalRHubClientsAdapter : Talabeyah.OrderManagement.Domain.Interfaces.IHubClients
{
    private readonly Microsoft.AspNetCore.SignalR.IHubClients _hubClients;

    public SignalRHubClientsAdapter(Microsoft.AspNetCore.SignalR.IHubClients hubClients)
    {
        _hubClients = hubClients;
    }

    public Talabeyah.OrderManagement.Domain.Interfaces.IClientProxy Group(string groupName)
    {
        return new SignalRClientProxyAdapter(_hubClients.Group(groupName));
    }

    public Talabeyah.OrderManagement.Domain.Interfaces.IClientProxy Groups(params string[] groupNames)
    {
        return new SignalRClientProxyAdapter(_hubClients.Groups(groupNames));
    }
}

public class SignalRClientProxyAdapter : Talabeyah.OrderManagement.Domain.Interfaces.IClientProxy
{
    private readonly Microsoft.AspNetCore.SignalR.IClientProxy _clientProxy;

    public SignalRClientProxyAdapter(Microsoft.AspNetCore.SignalR.IClientProxy clientProxy)
    {
        _clientProxy = clientProxy;
    }

    public Task SendAsync(string method, object? arg1, CancellationToken cancellationToken = default)
    {
        return _clientProxy.SendAsync(method, arg1, cancellationToken);
    }
} 
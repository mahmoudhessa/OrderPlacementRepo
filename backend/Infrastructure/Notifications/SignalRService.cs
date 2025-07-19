using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Infrastructure.Notifications;

public class SignalRService : ISignalRService
{
    private readonly IHubContext _hubContext;
    private readonly ILogger<SignalRService> _logger;

    public SignalRService(IHubContext hubContext, ILogger<SignalRService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendToGroupAsync(string groupName, string method, object message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.Group(groupName).SendAsync(method, message, cancellationToken);
            _logger.LogInformation("SignalR message sent to group {GroupName} with method {Method}", groupName, method);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SignalR message to group {GroupName} with method {Method}", groupName, method);
            throw;
        }
    }

    public async Task SendToGroupsAsync(string[] groupNames, string method, object message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.Groups(groupNames).SendAsync(method, message, cancellationToken);
            _logger.LogInformation("SignalR message sent to groups {GroupNames} with method {Method}", string.Join(", ", groupNames), method);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SignalR message to groups {GroupNames} with method {Method}", string.Join(", ", groupNames), method);
            throw;
        }
    }
} 
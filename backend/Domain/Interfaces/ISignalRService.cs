namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface ISignalRService
{
    Task SendToGroupAsync(string groupName, string method, object message, CancellationToken cancellationToken = default);
    Task SendToGroupsAsync(string[] groupNames, string method, object message, CancellationToken cancellationToken = default);
} 
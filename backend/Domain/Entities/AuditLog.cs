namespace Talabeyah.OrderManagement.Domain.Entities;

public class AuditLog
{
    public int Id { get; private set; }
    public string Change { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AuditLog(string change)
    {
        Change = change;
        CreatedAt = DateTime.UtcNow;
    }
} 
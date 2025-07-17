using Talabeyah.OrderManagement.Domain.Entities;

namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
} 
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Infrastructure.Persistence;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly OrderManagementDbContext _context;

    public AuditLogRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(AuditLog log)
    {
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
} 
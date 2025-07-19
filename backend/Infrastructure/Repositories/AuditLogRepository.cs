using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Infrastructure.Persistence;

namespace Talabeyah.OrderManagement.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly OrderManagementDbContext _db;
    public AuditLogRepository(OrderManagementDbContext db) => _db = db;

    public async Task AddAsync(AuditLog log)
    {
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetAllAsync()
        => await _db.AuditLogs.ToListAsync();
} 
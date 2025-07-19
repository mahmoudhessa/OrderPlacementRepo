using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Infrastructure.Repositories;

namespace Talabeyah.OrderManagement.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderManagementDbContext _context;
    
    public UnitOfWork(OrderManagementDbContext context)
    {
        _context = context;
        OrderRepository = new OrderRepository(context);
        ProductRepository = new ProductRepository(context);
        AuditLogRepository = new AuditLogRepository(context);
    }

    public IOrderRepository OrderRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IAuditLogRepository AuditLogRepository { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransaction(transaction);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public class EfTransaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public EfTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
} 
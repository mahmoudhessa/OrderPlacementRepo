namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface ITransaction : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork : IDisposable
{
    IOrderRepository OrderRepository { get; }
    IProductRepository ProductRepository { get; }
    IAuditLogRepository AuditLogRepository { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
} 
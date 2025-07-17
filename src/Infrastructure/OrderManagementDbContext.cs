using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;

namespace Talabeyah.OrderManagement.Infrastructure;

public class OrderManagementDbContext : DbContext
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasKey(p => p.Id);
        modelBuilder.Entity<Order>().HasKey(o => o.Id);
        modelBuilder.Entity<AuditLog>().HasKey(a => a.Id);
        // Configure OrderItem as owned entity
        modelBuilder.Entity<Order>().OwnsMany(o => o.Items, oi =>
        {
            oi.WithOwner().HasForeignKey("OrderId");
            oi.Property<int>("Id");
            oi.HasKey("Id");
        });
    }
} 
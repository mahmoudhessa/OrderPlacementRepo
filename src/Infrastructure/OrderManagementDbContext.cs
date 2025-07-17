using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Infrastructure.Entities;

namespace Talabeyah.OrderManagement.Infrastructure;

public class OrderManagementDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().HasKey(p => p.Id);
        modelBuilder.Entity<Order>().HasKey(o => o.Id);
        modelBuilder.Entity<AuditLog>().HasKey(a => a.Id);
        modelBuilder.Entity<Product>().Property(p => p.RowVersion).IsRowVersion();
        // Configure OrderItem as owned entity
        modelBuilder.Entity<Order>().OwnsMany(o => o.Items, oi =>
        {
            oi.WithOwner().HasForeignKey("OrderId");
            oi.Property<int>("Id");
            oi.HasKey("Id");
        });
    }
} 
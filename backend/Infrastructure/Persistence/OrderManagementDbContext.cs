using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
// using Talabeyah.OrderManagement.Infrastructure.Entities; // No longer needed

namespace Talabeyah.OrderManagement.Infrastructure.Persistence;

public class OrderManagementDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // Suppress the pending model changes warning
        optionsBuilder.ConfigureWarnings(warnings => warnings
            .Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().HasKey(p => p.Id);
        modelBuilder.Entity<Order>().HasKey(o => o.Id);
        modelBuilder.Entity<Order>().HasIndex(o => new { o.Status, o.CreatedAt });
        modelBuilder.Entity<AuditLog>().HasKey(a => a.Id);
        modelBuilder.Entity<Product>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<Order>().Property(o => o.RowVersion).IsRowVersion();
        // Configure OrderItem as owned entity
        modelBuilder.Entity<Order>().OwnsMany(o => o.Items, oi =>
        {
            oi.WithOwner().HasForeignKey("OrderId");
            oi.Property<int>("Id");
            oi.HasKey("Id");
            oi.HasIndex("ProductId");
            // Foreign key integrity for ProductId
            oi.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Restrict);
        });
    }
} 
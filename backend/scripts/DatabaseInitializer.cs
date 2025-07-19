using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Talabeyah.OrderManagement.Infrastructure.Persistence;

namespace DatabaseInitializer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Database Initializer Starting...");
        
        // Wait for SQL Server to be ready
        Console.WriteLine("Waiting for SQL Server to be ready...");
        await WaitForSqlServer();
        
        // Create host and apply migrations
        var host = CreateHostBuilder(args).Build();
        
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
        
        Console.WriteLine("Applying database migrations...");
        await context.Database.MigrateAsync();
        
        Console.WriteLine("Database initialization completed!");
    }
    
    static async Task WaitForSqlServer()
    {
        var maxRetries = 60;
        for (int i = 1; i <= maxRetries; i++)
        {
            try
            {
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(
                    "Server=sqlserver;Database=master;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;");
                await connection.OpenAsync();
                Console.WriteLine("SQL Server is ready!");
                return;
            }
            catch
            {
                Console.WriteLine($"Waiting for SQL Server... ({i}/{maxRetries})");
                await Task.Delay(1000);
            }
        }
        throw new Exception("SQL Server did not become ready in time");
    }
    
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<OrderManagementDbContext>(options =>
                    options.UseSqlServer("Server=sqlserver;Database=OrderManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"));
            });
} 
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Domain.ValueObjects;

namespace Talabeyah.OrderManagement.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(OrderManagementDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        // Ensure database exists and is up to date
        var maxRetries = 5;
        var retryCount = 0;
        
        while (retryCount < maxRetries)
        {
            try
            {
                // Ensure database exists
                await context.Database.EnsureCreatedAsync();
                
                // Try to apply migrations, but handle the case where tables already exist
                try
                {
                    await context.Database.MigrateAsync();
                }
                catch (Exception ex) when (ex.Message.Contains("already an object named") || ex.Message.Contains("already exists"))
                {
                    // If tables already exist, that's fine - the database is already set up
                    Console.WriteLine("Database tables already exist, skipping migration application");
                    break;
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("pending changes"))
                {
                    // If there are pending changes, just ensure the database is created
                    Console.WriteLine("Warning: Database model has pending changes, using EnsureCreated instead of migrations");
                }
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw new Exception($"Failed to initialize database after {maxRetries} attempts", ex);
                }
                await Task.Delay(2000 * retryCount);
            }
        }

        Console.WriteLine("Starting database seeding...");

        // Seed Roles
        var roles = new[] { "Admin", "InventoryManager", "Buyer" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole { Name = roleName };
                await roleManager.CreateAsync(role);
                Console.WriteLine($"Created role: {roleName}");
            }
        }

        // Seed Users
        var users = new[]
        {
            new { Email = "admin@demo.com", Password = "Passw0rd!", Role = "Admin" },
            new { Email = "inventory@demo.com", Password = "Passw0rd!", Role = "InventoryManager" },
            new { Email = "buyer@demo.com", Password = "Passw0rd!", Role = "Buyer" }
        };

        foreach (var userInfo in users)
        {
            var user = await userManager.FindByEmailAsync(userInfo.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userInfo.Email,
                    Email = userInfo.Email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, userInfo.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userInfo.Role);
                    Console.WriteLine($"Created user: {userInfo.Email} with role: {userInfo.Role}");
                }
            }
            else
            {
                // Check if user already has the role
                var userRoles = await userManager.GetRolesAsync(user);
                if (!userRoles.Contains(userInfo.Role))
                {
                    await userManager.AddToRoleAsync(user, userInfo.Role);
                    Console.WriteLine($"Added role {userInfo.Role} to existing user: {userInfo.Email}");
                }
                else
                {
                    Console.WriteLine($"User {userInfo.Email} already has role: {userInfo.Role}");
                }
            }
        }

        // Seed Products
        var products = new[]
        {
            new Product { Name = "Tide Detergent", Inventory = 120 },
            new Product { Name = "Nescafé Classic", Inventory = 85 },
            new Product { Name = "Always Ultra Pads", Inventory = 200 },
            new Product { Name = "Dettol Antiseptic", Inventory = 50 },
            new Product { Name = "Coca-Cola Can", Inventory = 300 }
        };

        foreach (var product in products)
        {
            if (!await context.Products.AnyAsync(p => p.Name == product.Name))
            {
                context.Products.Add(product);
                Console.WriteLine($"Added product: {product.Name}");
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine("Products seeded successfully.");

        // Seed Sample Orders
        var adminUser = await userManager.FindByEmailAsync("admin@demo.com");
        var buyerUser = await userManager.FindByEmailAsync("buyer@demo.com");
        var seededProducts = await context.Products.Take(3).ToListAsync();

        if (adminUser != null && buyerUser != null && seededProducts.Any())
        {
            var order1 = new Order
            {
                BuyerId = adminUser.Id,
                Status = OrderStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                Items = new List<OrderItem>
                {
                    new OrderItem(seededProducts[0].Id, 2),
                    new OrderItem(seededProducts[1].Id, 1)
                }
            };

            var order2 = new Order
            {
                BuyerId = buyerUser.Id,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddMinutes(-45),
                Items = new List<OrderItem>
                {
                    new OrderItem(seededProducts[2].Id, 3)
                }
            };

            var order3 = new Order
            {
                BuyerId = adminUser.Id,
                Status = OrderStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Items = new List<OrderItem>
                {
                    new OrderItem(seededProducts[0].Id, 1),
                    new OrderItem(seededProducts[1].Id, 2),
                    new OrderItem(seededProducts[2].Id, 1)
                }
            };

            var orders = new[] { order1, order2, order3 };

            foreach (var order in orders)
            {
                if (!await context.Orders.AnyAsync(o => o.BuyerId == order.BuyerId))
                {
                    context.Orders.Add(order);
                    Console.WriteLine($"Added order for user: {order.BuyerId} with {order.Items.Count} items");
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine("Sample orders seeded successfully.");
        }

        Console.WriteLine("Database initialization completed successfully.");
    }
} 
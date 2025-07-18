using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Infrastructure.Entities;

namespace Talabeyah.OrderManagement.Infrastructure;

public static class DbInitializer
{
    public static async Task SeedAsync(OrderManagementDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        await context.Database.MigrateAsync();

        // Seed roles
        var roles = new[] { "Admin", "Sales", "Auditor", "InventoryManager" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }

        // Seed users
        await SeedUserAsync(userManager, "admin@demo.com", "Passw0rd!", "Admin");
        await SeedUserAsync(userManager, "sales@demo.com", "Passw0rd!", "Sales");
        await SeedUserAsync(userManager, "auditor@demo.com", "Passw0rd!", "Auditor");
        await SeedUserAsync(userManager, "inventory@demo.com", "Passw0rd!", "InventoryManager");

        // Seed products
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product(1, "Tide Detergent", 120),
                new Product(2, "Nescafé Classic", 85),
                new Product(3, "Always Ultra Pads", 200),
                new Product(4, "Dettol Antiseptic", 50),
                new Product(5, "Coca-Cola Can", 300)
            );
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
    }
} 
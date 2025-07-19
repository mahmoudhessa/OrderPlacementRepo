using Microsoft.AspNetCore.Identity;

namespace Talabeyah.OrderManagement.Domain.Entities;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
} 
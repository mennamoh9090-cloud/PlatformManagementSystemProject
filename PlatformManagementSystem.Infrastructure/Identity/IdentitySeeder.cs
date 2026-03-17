using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Infrastructure.Identity;

public static class IdentitySeeder
{
    private static readonly string[] _roles = ["Student", "Instructor", "Admin"];

    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in _roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@test.com";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin"
            };

            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            // Ensure existing admin gets the Admin role if they don't have it
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
            {
                await userManager.AddToRoleAsync(existingAdmin, "Admin");
            }
            if (await userManager.IsInRoleAsync(existingAdmin, "Instructor"))
            {
                await userManager.RemoveFromRoleAsync(existingAdmin, "Instructor");
            }
        }

        // Add a brand new admin just in case
        var newAdminEmail = "superadmin@admin.com";
        if (await userManager.FindByEmailAsync(newAdminEmail) == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = newAdminEmail,
                Email = newAdminEmail,
                FullName = "Super Admin"
            };

            await userManager.CreateAsync(newAdmin, "Superadmin@123");
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}
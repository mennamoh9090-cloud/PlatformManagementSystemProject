using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PlatformManagementSystem.Domain.Entities;

public static class IdentitySeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "Instructor", "Student" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed Admin user
        var adminEmail = "admin@test.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Instructor" });
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed Instructor user
        var instructorEmail = "instructor@test.com";
        var instructorUser = await userManager.FindByEmailAsync(instructorEmail);
        if (instructorUser == null)
        {
            instructorUser = new ApplicationUser
            {
                UserName = instructorEmail,
                Email = instructorEmail,
                FullName = "Prof. Sarah Jenkins",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(instructorUser, "Instructor@123");
            await userManager.AddToRoleAsync(instructorUser, "Instructor");
        }

        // Seed Student user
        var studentEmail = "student@test.com";
        var studentUser = await userManager.FindByEmailAsync(studentEmail);
        if (studentUser == null)
        {
            studentUser = new ApplicationUser
            {
                UserName = studentEmail,
                Email = studentEmail,
                FullName = "Alex Morgan",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(studentUser, "Student@123");
            await userManager.AddToRoleAsync(studentUser, "Student");
        }
    }
}
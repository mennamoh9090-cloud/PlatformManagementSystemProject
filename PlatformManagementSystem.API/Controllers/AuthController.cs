using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.Application.DTOs.Auth;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtService jwtService) : ControllerBase
{
    //  LOGIN 
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return Unauthorized();

        var validPassword = await userManager.CheckPasswordAsync(user, model.Password);

        if (!validPassword)
            return Unauthorized();

        var roles = await userManager.GetRolesAsync(user);
        var role = "Student"; // Default fallback
        
        if (roles.Contains("Admin"))
            role = "Admin";
        else if (roles.Contains("Instructor"))
            role = "Instructor";
        else if (roles.Any())
            role = roles.First();

        var token = jwtService.GenerateToken(user, role);

        return Ok(new
        {
            token,
            email = user.Email,
            role,
            userId = user.Id
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
            return NotFound("User not found");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var result = await userManager.ResetPasswordAsync(user, token, "Instructor@123");

        if (result.Succeeded)
            return Ok("Password reset successfully");

        return BadRequest(result.Errors);
    }

    // REGISTER 
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            FullName = model.FullName,
            Email = model.Email,
            UserName = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await userManager.AddToRoleAsync(user, model.Role);

        return Ok("User Created");
    }
}
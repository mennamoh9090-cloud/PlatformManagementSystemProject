using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using PlatformManagementSystem.MVC.Helpers;

var builder = WebApplication.CreateBuilder(args);

#region Controllers

builder.Services.AddControllersWithViews();

#endregion

#region Session

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

#endregion

#region Authentication (Cookies)

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();

#endregion

#region HttpContext

builder.Services.AddHttpContextAccessor();

#endregion

#region HttpClient (API Connection)

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddTransient<JwtHandler>();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
})
.AddHttpMessageHandler<JwtHandler>();   

#endregion

#region Custom Services

builder.Services.AddScoped<IApiService, ApiService>();   
#endregion

var app = builder.Build();

#region Middleware Pipeline



app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

#endregion

#region Routes

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#endregion

app.Run();
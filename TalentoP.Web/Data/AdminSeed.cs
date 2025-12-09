using Microsoft.AspNetCore.Identity;

namespace TalentoP.Web.Data;

public static class AdminSeed
{
    public static async Task CreateDefaultAdminAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var adminEmail = "admin@talentoplus.com";
        var adminPassword = "admin123";

        var user = await userManager.FindByNameAsync(adminEmail);
        if (user != null) return;

        user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, adminPassword);
    }
}
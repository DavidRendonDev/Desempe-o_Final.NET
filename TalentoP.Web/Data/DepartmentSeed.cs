using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TalentoP.Web.Data;

public static class DepartmentSeed
{
    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // if there is at least 1 department, do nothing
        if (await db.Departments.AnyAsync()) return;

        db.Departments.AddRange(
            new Domain.Entities.Department { Name = "Technology" },
            new Domain.Entities.Department { Name = "Human Resources" },
            new Domain.Entities.Department { Name = "Finance" },
            new Domain.Entities.Department { Name = "Operations" }
        );

        await db.SaveChangesAsync();
    }
}
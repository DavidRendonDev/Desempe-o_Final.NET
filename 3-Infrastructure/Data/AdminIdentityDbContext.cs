using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AdminIdentityDbContext : IdentityDbContext
{
    public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options)
        : base(options)
    {
    }
}
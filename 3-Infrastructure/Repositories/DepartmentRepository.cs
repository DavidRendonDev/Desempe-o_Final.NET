using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _db;

    public DepartmentRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Department>> GetAllAsync()
    {
        return _db.Departments.OrderBy(d => d.Name).ToListAsync();
    }

    public Task<bool> ExistsAsync(int departmentId)
    {
        return _db.Departments.AnyAsync(d => d.Id == departmentId);
    }
}
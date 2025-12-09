using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Employee?> GetByIdAsync(int id)
    {
        return _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<Employee?> GetByDocumentAndEmailAsync(string documentNumber, string email)
    {
        return _db.Employees.FirstOrDefaultAsync(e =>
            e.DocumentNumber == documentNumber && e.Email == email);
    }

    public Task<Employee?> GetByDocumentAsync(string documentNumber)
    {
        return _db.Employees.FirstOrDefaultAsync(e => e.DocumentNumber == documentNumber);
    }

    public async Task AddAsync(Employee employee)
    {
        await _db.Employees.AddAsync(employee);
    }

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}
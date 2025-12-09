using Domain.Entities;

namespace Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByDocumentAndEmailAsync(string documentNumber, string email);
    Task<Employee?> GetByDocumentAsync(string documentNumber);
    Task AddAsync(Employee employee);
    Task SaveChangesAsync();
}
using Domain.Entities;

namespace Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();
    Task<bool> ExistsAsync(int departmentId);
}
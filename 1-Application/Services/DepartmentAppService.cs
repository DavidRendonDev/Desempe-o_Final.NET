using Application.DTOs.Departments;
using Domain.Interfaces;

namespace Application.Services;

public class DepartmentAppService
{
    private readonly IDepartmentRepository _departments;

    public DepartmentAppService(IDepartmentRepository departments)
    {
        _departments = departments;
    }

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        var list = await _departments.GetAllAsync();

        return list.Select(d => new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name
        }).ToList();
    }
}
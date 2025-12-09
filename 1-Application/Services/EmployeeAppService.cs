using Application.DTOs.Employees;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class EmployeeAppService
{
    private readonly IEmployeeRepository _employees;
    private readonly IPdfService _pdf;

    public EmployeeAppService(IEmployeeRepository employees, IPdfService pdf)
    {
        _employees = employees;
        _pdf = pdf;
    }

    public async Task<EmployeeMeDto?> GetMeAsync(int employeeId)
    {
        var employee = await _employees.GetByIdAsync(employeeId);
        if (employee == null) return null;

        return new EmployeeMeDto
        {
            Id = employee.Id,
            DocumentNumber = employee.DocumentNumber,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            JobTitle = employee.JobTitle,
            Salary = employee.Salary,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            EducationLevel = employee.EducationLevel,
            ProfessionalProfile = employee.ProfessionalProfile,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name ?? ""
        };
    }

    public async Task<(byte[]? pdfBytes, string fileName)> GetMyResumePdfAsync(int employeeId)
    {
        var employee = await _employees.GetByIdAsync(employeeId);
        if (employee == null) return (null, "");

        var bytes = _pdf.GenerateEmployeeResumePdf(employee);
        var fileName = $"Resume_{employee.DocumentNumber}.pdf";

        return (bytes, fileName);
    }
}
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AuthAppService
{
    private readonly IEmployeeRepository _employees;
    private readonly IDepartmentRepository _departments;
    private readonly IJwtService _jwt;
    private readonly IEmailService _email;

    public AuthAppService(
        IEmployeeRepository employees,
        IDepartmentRepository departments,
        IJwtService jwt,
        IEmailService email)
    {
        _employees = employees;
        _departments = departments;
        _jwt = jwt;
        _email = email;
    }

    public async Task<(bool ok, string message)> RegisterAsync(RegisterEmployeeDto dto)
    {
        var deptOk = await _departments.ExistsAsync(dto.DepartmentId);
        if (!deptOk) return (false, "Invalid DepartmentId.");

        var exists = await _employees.GetByDocumentAsync(dto.DocumentNumber);
        if (exists != null) return (false, "Employee with this DocumentNumber already exists.");

        var employee = new Employee
        {
            DocumentNumber = dto.DocumentNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DepartmentId = dto.DepartmentId,

            // defaults
            EmploymentStatus = "Inactive",
            JobTitle = "Pending",
            Salary = 0,
            HireDate = DateTime.Today,
            EducationLevel = "",
            ProfessionalProfile = ""
        };

        await _employees.AddAsync(employee);
        await _employees.SaveChangesAsync();

        await _email.SendWelcomeEmailAsync(employee.Email, $"{employee.FirstName} {employee.LastName}");

        return (true, "Registration successful. Welcome email sent.");
    }

    public async Task<(bool ok, string message, LoginResultDto? result)> LoginAsync(LoginEmployeeDto dto)
    {
        var employee = await _employees.GetByDocumentAndEmailAsync(dto.DocumentNumber, dto.Email);
        if (employee == null) return (false, "Invalid credentials.", null);

        var token = _jwt.CreateEmployeeToken(employee.Id, employee.Email, employee.DocumentNumber);

        return (true, "Login ok.", new LoginResultDto
        {
            Token = token,
            ExpiresMinutes = _jwt.GetExpiresMinutes()
        });
    }
}

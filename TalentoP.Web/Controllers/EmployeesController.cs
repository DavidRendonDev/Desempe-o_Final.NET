using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TalentoP.Web.ViewModels;
using QuestPDF.Fluent;
using TalentoP.Web.Pdf;


namespace TalentoP.Web.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _db.Employees
            .Include(e => e.Department)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();

        return View(employees);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadDepartmentsAsync();
        return View(new EmployeeFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync();
            return View(vm);
        }

        var exists = await _db.Employees.AnyAsync(e => e.DocumentNumber == vm.DocumentNumber);
        if (exists)
        {
            ModelState.AddModelError(nameof(vm.DocumentNumber), "DocumentNumber already exists.");
            await LoadDepartmentsAsync();
            return View(vm);
        }

        var employee = new Domain.Entities.Employee
        {
            DocumentNumber = vm.DocumentNumber,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Phone = vm.Phone,
            JobTitle = vm.JobTitle,
            Salary = vm.Salary,
            HireDate = vm.HireDate,
            EmploymentStatus = vm.EmploymentStatus,
            EducationLevel = vm.EducationLevel,
            ProfessionalProfile = vm.ProfessionalProfile,
            DepartmentId = vm.DepartmentId
        };

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return NotFound();

        var vm = new EmployeeFormViewModel
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
            DepartmentId = employee.DepartmentId
        };

        await LoadDepartmentsAsync();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EmployeeFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync();
            return View(vm);
        }

        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == vm.Id);
        if (employee == null) return NotFound();

        // if document changes, validate uniqueness
        var docExists = await _db.Employees.AnyAsync(e => e.DocumentNumber == vm.DocumentNumber && e.Id != employee.Id);
        if (docExists)
        {
            ModelState.AddModelError(nameof(vm.DocumentNumber), "DocumentNumber already exists.");
            await LoadDepartmentsAsync();
            return View(vm);
        }

        employee.DocumentNumber = vm.DocumentNumber;
        employee.FirstName = vm.FirstName;
        employee.LastName = vm.LastName;
        employee.Email = vm.Email;
        employee.Phone = vm.Phone;
        employee.JobTitle = vm.JobTitle;
        employee.Salary = vm.Salary;
        employee.HireDate = vm.HireDate;
        employee.EmploymentStatus = vm.EmploymentStatus;
        employee.EducationLevel = vm.EducationLevel;
        employee.ProfessionalProfile = vm.ProfessionalProfile;
        employee.DepartmentId = vm.DepartmentId;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        return View(employee);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return NotFound();

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDepartmentsAsync()
    {
        var departments = await _db.Departments
            .OrderBy(d => d.Name)
            .ToListAsync();

        ViewBag.Departments = departments
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();
    }
    
    [HttpGet]
    public async Task<IActionResult> ResumePdf(int id)
    {
        var employee = await _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        var document = new EmployeeResumeDocument(employee);
        var pdfBytes = document.GeneratePdf();

        var fileName = $"Resume_{employee.DocumentNumber}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

}

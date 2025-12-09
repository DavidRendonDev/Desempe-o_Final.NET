using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoP.Web.Services;

namespace TalentoP.Web.Controllers;

[Authorize]
public class ImportController : Controller
{
    private readonly EmployeeExcelImportService _service;

    public ImportController(EmployeeExcelImportService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Employees()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Employees(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.Error = "Please select an Excel file.";
            return View();
        }

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".xlsx")
        {
            ViewBag.Error = "Only .xlsx files are allowed.";
            return View();
        }

        using var stream = file.OpenReadStream();
        var result = await _service.ImportAsync(stream);

        if (!result.ok)
        {
            ViewBag.Error = result.message;
            return View();
        }

        ViewBag.Success = $"{result.message} Created: {result.created}, Updated: {result.updated}";
        return View();
    }
}
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoP.Web.Services;


namespace TalentoP.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var totalEmployees = await _db.Employees.CountAsync();

        var vacationEmployees = await _db.Employees
            .CountAsync(e => e.EmploymentStatus == "Vacation");

        var activeEmployees = await _db.Employees
            .CountAsync(e => e.EmploymentStatus == "Active");

        ViewBag.TotalEmployees = totalEmployees;
        ViewBag.VacationEmployees = vacationEmployees;
        ViewBag.ActiveEmployees = activeEmployees;

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> AiQuery(string question, [FromServices] AiQueryService ai)
    {
        if (string.IsNullOrWhiteSpace(question))
            return Json(new { ok = false, message = "Question is required." });

        try
        {
            var answer = await ai.AskAsync(question);
            return Json(new { ok = true, answer });
        }
        catch (Exception ex)
        {
            return Json(new { ok = false, message = ex.Message });
        }
    }

}
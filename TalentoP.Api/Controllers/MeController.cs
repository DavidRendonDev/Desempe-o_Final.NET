using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TalentoP.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly EmployeeAppService _service;

    public MeController(EmployeeAppService service)
    {
        _service = service;
    }

    [HttpGet("resume/pdf")]
    public async Task<IActionResult> GetMyResumePdf()
    {
        var idText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idText, out var employeeId)) return Unauthorized();

        var result = await _service.GetMyResumePdfAsync(employeeId);
        if (result.pdfBytes == null) return NotFound();

        return File(result.pdfBytes, "application/pdf", result.fileName);
    }
}
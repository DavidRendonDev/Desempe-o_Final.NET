using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace TalentoP.Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly DepartmentAppService _service;

    public DepartmentsController(DepartmentAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }
}
using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace TalentoP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthAppService _service;

    public AuthController(AuthAppService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterEmployeeDto dto)
    {
        var result = await _service.RegisterAsync(dto);
        if (!result.ok) return BadRequest(new { message = result.message });
        return Ok(new { message = result.message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginEmployeeDto dto)
    {
        var result = await _service.LoginAsync(dto);
        if (!result.ok) return Unauthorized(new { message = result.message });
        return Ok(result.result);
    }
}
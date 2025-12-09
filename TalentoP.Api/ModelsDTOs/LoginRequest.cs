using System.ComponentModel.DataAnnotations;

namespace TalentoP.Api.Models;

public class LoginRequest
{
    [Required, MaxLength(30)]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;
}
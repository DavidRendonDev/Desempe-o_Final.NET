using System.ComponentModel.DataAnnotations;

namespace TalentoP.Api.Models;

public class RegisterEmployeeRequest
{
    [Required, MaxLength(30)]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }
}
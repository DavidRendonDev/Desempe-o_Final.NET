using System.ComponentModel.DataAnnotations;

namespace TalentoP.Web.ViewModels;

public class EmployeeFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(120)]
    public string JobTitle { get; set; } = string.Empty;

    [Range(0, 999999999)]
    public decimal Salary { get; set; }

    [Required]
    public DateTime HireDate { get; set; } = DateTime.Today;

    [Required]
    [MaxLength(30)]
    public string EmploymentStatus { get; set; } = "Active";

    [MaxLength(120)]
    public string EducationLevel { get; set; } = string.Empty;

    public string ProfessionalProfile { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }
}
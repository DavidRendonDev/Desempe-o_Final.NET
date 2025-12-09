namespace Domain.Entities;

public class Employee
{
    public int Id { get; set; }

    // Unique identifier from Excel (usually)
    public string DocumentNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Work info
    public string JobTitle { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }

    // Simple status (keep junior-friendly)
    public string EmploymentStatus { get; set; } = "Active"; // Active / Inactive / Vacation

    // Education / profile
    public string EducationLevel { get; set; } = string.Empty;
    public string ProfessionalProfile { get; set; } = string.Empty;

    // Department relation
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
}
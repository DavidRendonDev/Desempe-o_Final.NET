namespace Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation (optional but useful)
    public List<Employee> Employees { get; set; } = new();
}
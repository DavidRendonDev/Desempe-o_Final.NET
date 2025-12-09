namespace TalentoP.Web.Ai;

public class AiQueryIntent
{
    public string Action { get; set; } = "countEmployees";

    // Filters
    public string? Department { get; set; }          
    public string? EmploymentStatus { get; set; }    
    public string? JobTitleContains { get; set; }    
}
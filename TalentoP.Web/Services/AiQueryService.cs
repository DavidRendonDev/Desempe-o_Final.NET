using System.Text.Json;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TalentoP.Web.Ai;

namespace TalentoP.Web.Services;

public class AiQueryService
{
    private readonly AppDbContext _db;
    private readonly GeminiClient _gemini;

    public AiQueryService(AppDbContext db, GeminiClient gemini)
    {
        _db = db;
        _gemini = gemini;
    }

    public async Task<string> AskAsync(string question)
    {
        var prompt = BuildPrompt(question);

        var raw = await _gemini.GenerateAsync(prompt);

        // Gemini might return extra text; extract JSON safely
        var json = ExtractJson(raw);

        var intent = JsonSerializer.Deserialize<AiQueryIntent>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (intent == null)
            return "I couldn't understand the question.";

        return await ExecuteIntentAsync(intent);
    }

    private string BuildPrompt(string question)
    {
        // IMPORTANT: we force JSON only
        return $@"
You are an assistant that converts HR questions into a JSON intent.
Return ONLY valid JSON. No explanations. No markdown.

Supported action:
- countEmployees

Fields:
- action: string
- department: string|null
- employmentStatus: string|null  (Active, Inactive, Vacation)
- jobTitleContains: string|null

Examples:
Question: ""How many employees are in Technology?""
JSON: {{ ""action"": ""countEmployees"", ""department"": ""Technology"", ""employmentStatus"": null, ""jobTitleContains"": null }}

Question: ""How many inactive employees?""
JSON: {{ ""action"": ""countEmployees"", ""department"": null, ""employmentStatus"": ""Inactive"", ""jobTitleContains"": null }}

Question: ""How many auxiliaries are there?""
JSON: {{ ""action"": ""countEmployees"", ""department"": null, ""employmentStatus"": null, ""jobTitleContains"": ""Auxiliar"" }}

Now convert this question:
""{question}""
";
    }

    private static string ExtractJson(string text)
    {
        // Very simple extraction: first '{' to last '}'
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');

        if (start < 0 || end < 0 || end <= start)
            return "{}";

        return text.Substring(start, end - start + 1);
    }

    private async Task<string> ExecuteIntentAsync(AiQueryIntent intent)
    {
        if (!string.Equals(intent.Action, "countEmployees", StringComparison.OrdinalIgnoreCase))
            return "This action is not supported yet.";

        var query = _db.Employees
            .Include(e => e.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(intent.Department))
        {
            query = query.Where(e => e.Department != null && e.Department.Name == intent.Department);
        }

        if (!string.IsNullOrWhiteSpace(intent.EmploymentStatus))
        {
            query = query.Where(e => e.EmploymentStatus == intent.EmploymentStatus);
        }

        if (!string.IsNullOrWhiteSpace(intent.JobTitleContains))
        {
            query = query.Where(e => e.JobTitle.Contains(intent.JobTitleContains));
        }

        var count = await query.CountAsync();

        // Friendly answer
        return $"Result: {count}";
    }
}

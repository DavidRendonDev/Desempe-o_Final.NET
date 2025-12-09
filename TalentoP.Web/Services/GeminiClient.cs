using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TalentoP.Web.Services;

public class GeminiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public GeminiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var apiKey = _config["Gemini:ApiKey"];
        var model = _config["Gemini:Model"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("Gemini ApiKey is missing in appsettings.json");

        if (string.IsNullOrWhiteSpace(model))
            model = "gemini-2.5-flash";

        // Gemini API: models.generateContent
        // Docs: https://ai.google.dev/api/generate-content :contentReference[oaicite:2]{index=2}
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = prompt } }
                }
            }
        };

        var json = JsonSerializer.Serialize(body);
        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var res = await _http.SendAsync(req);
        var resText = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception($"Gemini API error: {res.StatusCode} - {resText}");

        // Parse: candidates[0].content.parts[0].text
        using var doc = JsonDocument.Parse(resText);

        var text =
            doc.RootElement
               .GetProperty("candidates")[0]
               .GetProperty("content")
               .GetProperty("parts")[0]
               .GetProperty("text")
               .GetString();

        return text ?? "";
    }
}

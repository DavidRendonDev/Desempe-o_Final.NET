namespace Application.DTOs.Auth;

public class LoginResultDto
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; }
}
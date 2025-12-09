namespace Application.Interfaces;

public interface IJwtService
{
    string CreateEmployeeToken(int employeeId, string email, string documentNumber);
    int GetExpiresMinutes();
}
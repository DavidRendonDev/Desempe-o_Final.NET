using Domain.Entities;

namespace Application.Interfaces;

public interface IPdfService
{
    byte[] GenerateEmployeeResumePdf(Employee employee);
}
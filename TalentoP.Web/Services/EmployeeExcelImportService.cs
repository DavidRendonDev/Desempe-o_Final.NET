using ClosedXML.Excel;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TalentoP.Web.Services;

public class EmployeeExcelImportService
{
    private readonly AppDbContext _db;

    public EmployeeExcelImportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool ok, string message, int created, int updated, int skipped)> ImportAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();

        // 1) Read headers in row 1
        var headerRow = ws.Row(1);
        var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

        // Map: excelHeader -> columnIndex
        var excelHeaders = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (int col = 1; col <= lastCol; col++)
        {
            var header = headerRow.Cell(col).GetString().Trim();
            if (!string.IsNullOrWhiteSpace(header) && !excelHeaders.ContainsKey(header))
                excelHeaders.Add(header, col);
        }

        // 2) Define mapping (Excel header text -> internal column name)
        // TODO: Replace left side with EXACT headers from your Excel
        var headerMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // ExcelHeader -> InternalName
            { "DocumentNumber", EmployeeExcelColumns.DocumentNumber },
            { "FirstName", EmployeeExcelColumns.FirstName },
            { "LastName", EmployeeExcelColumns.LastName },
            { "Email", EmployeeExcelColumns.Email },
            { "Phone", EmployeeExcelColumns.Phone },
            { "JobTitle", EmployeeExcelColumns.JobTitle },
            { "Salary", EmployeeExcelColumns.Salary },
            { "HireDate", EmployeeExcelColumns.HireDate },
            { "EmploymentStatus", EmployeeExcelColumns.EmploymentStatus },
            { "EducationLevel", EmployeeExcelColumns.EducationLevel },
            { "ProfessionalProfile", EmployeeExcelColumns.ProfessionalProfile },
            { "Department", EmployeeExcelColumns.Department }
        };

        // 3) Build internalName -> columnIndex
        var internalCols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in headerMap)
        {
            var excelHeader = kv.Key;
            var internalName = kv.Value;

            if (excelHeaders.TryGetValue(excelHeader, out var colIndex))
                internalCols[internalName] = colIndex;
        }

        // 4) Validate required columns
        var required = new[]
        {
            EmployeeExcelColumns.DocumentNumber,
            EmployeeExcelColumns.FirstName,
            EmployeeExcelColumns.LastName,
            EmployeeExcelColumns.Email,
            EmployeeExcelColumns.Department
        };

        var missing = required.Where(r => !internalCols.ContainsKey(r)).ToList();
        if (missing.Any())
        {
            return (false, "Missing required columns: " + string.Join(", ", missing), 0, 0, 0);
        }

        // 5) Iterate rows
        int created = 0, updated = 0, skipped = 0;
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

        for (int row = 2; row <= lastRow; row++)
        {
            var r = ws.Row(row);

            string GetText(string internalName)
            {
                if (!internalCols.TryGetValue(internalName, out var col)) return "";
                return r.Cell(col).GetString().Trim();
            }

            DateTime GetDate(string internalName)
            {
                if (!internalCols.TryGetValue(internalName, out var col)) return DateTime.Today;
                var cell = r.Cell(col);

                if (cell.DataType == XLDataType.DateTime)
                    return cell.GetDateTime();

                DateTime.TryParse(cell.GetString().Trim(), out var dt);
                return dt == default ? DateTime.Today : dt;
            }

            decimal GetDecimal(string internalName)
            {
                if (!internalCols.TryGetValue(internalName, out var col)) return 0;
                var text = r.Cell(col).GetString().Trim();
                decimal.TryParse(text, out var value);
                return value;
            }

            var documentNumber = GetText(EmployeeExcelColumns.DocumentNumber);
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                skipped++;
                continue;
            }

            var firstName = GetText(EmployeeExcelColumns.FirstName);
            var lastName = GetText(EmployeeExcelColumns.LastName);
            var email = GetText(EmployeeExcelColumns.Email);
            var departmentName = GetText(EmployeeExcelColumns.Department);

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(departmentName))
            {
                skipped++;
                continue;
            }

            // Optional fields
            var phone = GetText(EmployeeExcelColumns.Phone);
            var jobTitle = GetText(EmployeeExcelColumns.JobTitle);
            var status = GetText(EmployeeExcelColumns.EmploymentStatus);
            if (string.IsNullOrWhiteSpace(status)) status = "Active";

            var educationLevel = GetText(EmployeeExcelColumns.EducationLevel);
            var professionalProfile = GetText(EmployeeExcelColumns.ProfessionalProfile);

            var salary = GetDecimal(EmployeeExcelColumns.Salary);
            var hireDate = GetDate(EmployeeExcelColumns.HireDate);

            // Ensure department exists
            var dept = await _db.Departments.FirstOrDefaultAsync(d => d.Name == departmentName);
            if (dept == null)
            {
                dept = new Domain.Entities.Department { Name = departmentName };
                _db.Departments.Add(dept);
                await _db.SaveChangesAsync();
            }

            // Upsert employee
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.DocumentNumber == documentNumber);

            if (employee == null)
            {
                employee = new Domain.Entities.Employee { DocumentNumber = documentNumber };
                _db.Employees.Add(employee);
                created++;
            }
            else
            {
                updated++;
            }

            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.Email = email;
            employee.Phone = phone;
            employee.JobTitle = jobTitle;
            employee.Salary = salary;
            employee.HireDate = hireDate;
            employee.EmploymentStatus = status;
            employee.EducationLevel = educationLevel;
            employee.ProfessionalProfile = professionalProfile;
            employee.DepartmentId = dept.Id;
        }

        await _db.SaveChangesAsync();
        return (true, "Excel imported successfully.", created, updated, skipped);
    }
}

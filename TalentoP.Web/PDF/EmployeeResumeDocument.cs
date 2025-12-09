using Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TalentoP.Web.Pdf;

public class EmployeeResumeDocument : IDocument
{
    private readonly Employee _employee;

    public EmployeeResumeDocument(Employee employee)
    {
        _employee = employee;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Size(PageSizes.A4);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header().Element(HeaderSection);
            page.Content().Element(ContentSection);
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("TalentoPlus S.A.S - Employee Resume | Generated: ");
                x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            });
        });
    }

    private void HeaderSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("Employee Resume").FontSize(22).Bold();
            col.Item().Text($"{_employee.FirstName} {_employee.LastName}").FontSize(16).SemiBold();
            col.Item().LineHorizontal(1);
        });
    }

    private void ContentSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(10);

            col.Item().Element(c => SectionTitle(c, "Personal Information"));
            col.Item().Element(PersonalInfoTable);

            col.Item().Element(c => SectionTitle(c, "Work Information"));
            col.Item().Element(WorkInfoTable);

            col.Item().Element(c => SectionTitle(c, "Education"));
            col.Item().Text(EmptyIfNull(_employee.EducationLevel));

            col.Item().Element(c => SectionTitle(c, "Professional Profile"));
            col.Item().Text(EmptyIfNull(_employee.ProfessionalProfile));
        });
    }

    private void PersonalInfoTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(150);
                columns.RelativeColumn();
            });

            Row(table, "Document Number:", _employee.DocumentNumber);
            Row(table, "Email:", _employee.Email);
            Row(table, "Phone:", _employee.Phone);
        });
    }

    private void WorkInfoTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(150);
                columns.RelativeColumn();
            });

            Row(table, "Job Title:", _employee.JobTitle);
            Row(table, "Salary:", _employee.Salary.ToString("N2"));
            Row(table, "Hire Date:", _employee.HireDate.ToString("yyyy-MM-dd"));
            Row(table, "Status:", _employee.EmploymentStatus);
            Row(table, "Department:", _employee.Department?.Name ?? "-");
        });
    }

    private static void SectionTitle(IContainer container, string title)
    {
        container
            .Background(Colors.Grey.Lighten3)
            .Padding(6)
            .Text(title)
            .Bold();
    }

    private static void Row(TableDescriptor table, string label, string? value)
    {
        table.Cell().Element(CellStyle).Text(label).SemiBold();
        table.Cell().Element(CellStyle).Text(EmptyIfNull(value));
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4);
    }

    private static string EmptyIfNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "-" : value;
    }
}

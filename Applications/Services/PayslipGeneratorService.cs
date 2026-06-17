using Api.Contracts;
using Api.Contracts.Payroll;
using ApiHrm.Domains.Entities;
using Applications.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace Applications.Services
{
    public class PayslipGeneratorService : IPayslipGeneratorService
    {
        private readonly CompanySettings _companySettings;
        private readonly ILogger<PayslipGeneratorService> _logger;
        private readonly string _baseStoragePath;

        public PayslipGeneratorService(
            IOptions<CompanySettings> companySettings,
            ILogger<PayslipGeneratorService> logger)
        {
            _companySettings = companySettings.Value;
            _logger = logger;
            _baseStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Payslips");
        }

        public async Task<string> GeneratePayslipAsync(
            PayrollComputeResultDto computationResult,
            Employee employee,
            int payrollRunId,
            DateOnly payoutDate)
        {
            try
            {
                // Create directory if it doesn't exist
                var directory = Path.Combine(_baseStoragePath, payrollRunId.ToString());
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Generate file path
                var fileName = $"{employee.EmployeeId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
                var filePath = Path.Combine(directory, fileName);

                // Create PDF document
                var document = new PayslipDocument(
                    _companySettings,
                    computationResult,
                    employee,
                    payrollRunId,
                    payoutDate);

                document.GeneratePdf(filePath);

                _logger.LogInformation("Payslip generated successfully for EmployeeId {EmployeeId} at {FilePath}", 
                    employee.EmployeeId, filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating payslip for EmployeeId {EmployeeId}", employee.EmployeeId);
                return string.Empty;
            }
        }
    }

    public class PayslipDocument : IDocument
    {
        private readonly CompanySettings _companySettings;
        private readonly PayrollComputeResultDto _computationResult;
        private readonly Employee _employee;
        private readonly int _payrollRunId;
        private readonly DateOnly _payoutDate;

        public PayslipDocument(
            CompanySettings companySettings,
            PayrollComputeResultDto computationResult,
            Employee employee,
            int payrollRunId,
            DateOnly payoutDate)
        {
            _companySettings = companySettings;
            _computationResult = computationResult;
            _employee = employee;
            _payrollRunId = payrollRunId;
            _payoutDate = payoutDate;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(_companySettings.Name).Bold().FontSize(16);
                    column.Item().Text(_companySettings.Address).FontSize(9);
                    column.Item().Text($"Contact: {_companySettings.Contact} | Email: {_companySettings.Email}").FontSize(9);
                });

                row.ConstantItem(150).Column(column =>
                {
                    column.Item().AlignRight().Text("PAYSLIP").Bold().FontSize(14);
                    column.Item().AlignRight().Text($"Payroll Run: {_payrollRunId}").FontSize(9);
                    column.Item().AlignRight().Text($"Cutoff Date: {_computationResult.Cutoff_Date:yyyy-MM-dd}").FontSize(9);
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                // Employee Details Section
                column.Item().Element(ComposeEmployeeDetails);
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                column.Spacing(10);

                // Earnings Section
                column.Item().Element(ComposeEarnings);
                column.Spacing(10);

                // Deductions Section
                column.Item().Element(ComposeDeductions);
                column.Spacing(10);

                // Summary Section
                column.Item().Element(ComposeSummary);
            });
        }

        void ComposeEmployeeDetails(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(100);
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Employee Details").Bold();
                    header.Cell().Element(CellStyle);
                });

                table.Cell().Element(CellStyle).Text("Name:");
                table.Cell().Element(CellStyle).Text($"{_employee.FirstName} {_employee.LastName}");

                table.Cell().Element(CellStyle).Text("Employee ID:");
                table.Cell().Element(CellStyle).Text(_employee.EmployeeId.ToString());

                table.Cell().Element(CellStyle).Text("Position:");
                table.Cell().Element(CellStyle).Text(_employee.EmploymentDetails?.Position ?? "N/A");

                table.Cell().Element(CellStyle).Text("Payment Method:");
                table.Cell().Element(CellStyle).Text("Bank"); // Default payment method
            });
        }

        void ComposeEarnings(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.ConstantColumn(80);
                    columns.ConstantColumn(100);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Description").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Days/Hours").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Amount").Bold();
                });

                // Basic Pay
                table.Cell().Element(CellStyle).Text("Basic Pay");
                table.Cell().Element(CellStyle).AlignRight().Text(_computationResult.Days_Worked.ToString());
                table.Cell().Element(CellStyle).AlignRight().Text($"{_computationResult.Basic_Pay:C2}");

                // OT Pay
                table.Cell().Element(CellStyle).Text("Overtime Pay");
                table.Cell().Element(CellStyle).AlignRight().Text($"{_computationResult.OT_Hours:F1} hrs");
                table.Cell().Element(CellStyle).AlignRight().Text($"{_computationResult.OT_Pay:C2}");

                // Line Items (Bonuses/Incentives)
                foreach (var item in _computationResult.Line_Items)
                {
                    table.Cell().Element(CellStyle).Text(item.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text("-");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Amount:C2}");
                }

                // Total Earnings
                var totalEarnings = _computationResult.Basic_Pay + _computationResult.OT_Pay + 
                    _computationResult.Line_Items.Sum(x => x.Amount);
                table.Cell().Element(TotalCellStyle).Text("Total Earnings").Bold();
                table.Cell().Element(TotalCellStyle);
                table.Cell().Element(TotalCellStyle).AlignRight().Text($"{totalEarnings:C2}").Bold();
            });
        }

        void ComposeDeductions(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.ConstantColumn(100);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Deduction").Bold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Amount").Bold();
                });

                table.Cell().Element(CellStyle).Text("SSS");
                table.Cell().Element(CellStyle).AlignRight().Text($"{_computationResult.Sss_Deduction:C2}");

                table.Cell().Element(CellStyle).Text("PhilHealth");
                table.Cell().Element(CellStyle).AlignRight().Text($"{_computationResult.PhilHealth_Deduction:C2}");

                table.Cell().Element(CellStyle).Text("Pag-IBIG");
                table.Cell().Element(CellStyle).AlignRight().Text($"{_computationResult.PagIbig_Deduction:C2}");

                table.Cell().Element(CellStyle).Text("Tax");
                table.Cell().Element(CellStyle).AlignRight().Text($"{_computationResult.Tax_Deduction:C2}");

                // Total Deductions
                table.Cell().Element(TotalCellStyle).Text("Total Deductions").Bold();
                table.Cell().Element(TotalCellStyle).AlignRight().Text($"{_computationResult.Total_Deductions:C2}").Bold();
            });
        }

        void ComposeSummary(IContainer container)
        {
            container.Padding(10).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Lighten1).Column(column =>
            {
                column.Item().AlignCenter().Text("NET PAY").Bold().FontSize(14);
                column.Item().AlignCenter().Text($"{_computationResult.Net_Pay:C2}").Bold().FontSize(18);
                column.Spacing(5);
                column.Item().AlignCenter().Text($"Payout Date: {_payoutDate:yyyy-MM-dd}").FontSize(9);
            });
        }

        static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5)
                .AlignLeft();
        }

        static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Background(Colors.Grey.Lighten2)
                .Padding(5)
                .AlignLeft();
        }

        static IContainer TotalCellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Background(Colors.Grey.Lighten2)
                .Padding(5)
                .AlignLeft();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Api.Contracts.Payroll;
using Applications.Interfaces;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using System.Text.Json;

namespace ApiHrm.Controllers
{
    [Route("api/payroll")]
    [ApiController]
    [Authorize]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollComputationService _payrollComputationService;
        private readonly hrmAppDbContext _context;
        private readonly ILogger<PayrollController> _logger;
        private readonly IPayslipGeneratorService _payslipGeneratorService;

        public PayrollController(
            IPayrollComputationService payrollComputationService,
            hrmAppDbContext context,
            ILogger<PayrollController> logger,
            IPayslipGeneratorService payslipGeneratorService)
        {
            _payrollComputationService = payrollComputationService;
            _context = context;
            _logger = logger;
            _payslipGeneratorService = payslipGeneratorService;
        }

        /// <summary>
        /// Computes payroll for a single employee for the given payroll run.
        /// Returns a full breakdown (basic pay, OT, deductions, net pay) without persisting anything.
        /// </summary>
        [HttpGet("compute/{payrollRunId}/employee/{employeeId}")]
        [Authorize(Roles = "Admin,SystemAdmin,Manager,HR,HRAdmin")]
        public async Task<ActionResult<PayrollComputeResultDto>> Compute(
            [FromRoute] int payrollRunId,
            [FromRoute] int employeeId)
        {
            try
            {
                var payrollRun = await _context.PayrollRuns.FindAsync(payrollRunId);
                if (payrollRun == null)
                {
                    return NotFound($"Payroll run with ID {payrollRunId} not found.");
                }

                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null)
                {
                    return NotFound($"Employee with ID {employeeId} not found.");
                }

                var result = await _payrollComputationService.ComputePayrollAsync(payrollRunId, employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing payroll for payrollRunId {PayrollRunId}, employeeId {EmployeeId}", payrollRunId, employeeId);
                return BadRequest($"Error computing payroll: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds or updates a bonus/incentive for an employee in a payroll run.
        /// Rejects requests if the run is already finalized.
        /// </summary>
        [HttpPost("bonus")]
        [Authorize(Roles = "Admin,SystemAdmin,Manager,HR,HRAdmin")]
        public async Task<ActionResult> AddBonus([FromBody] BonusRequestDto dto)
        {
            try
            {
                var payrollRun = await _context.PayrollRuns.FindAsync(dto.Payroll_Run_Id);
                if (payrollRun == null)
                {
                    return NotFound($"Payroll run with ID {dto.Payroll_Run_Id} not found.");
                }

                if (payrollRun.Status == "Finalized")
                {
                    return BadRequest("Cannot add bonus to a finalized payroll run.");
                }

                var employee = await _context.Employees.FindAsync(dto.Employee_Id);
                if (employee == null)
                {
                    return NotFound($"Employee with ID {dto.Employee_Id} not found.");
                }

                var existingBonus = await _context.BonusIncentives
                    .FirstOrDefaultAsync(b => b.Payroll_Run_Id == dto.Payroll_Run_Id && b.Employee_Id == dto.Employee_Id);

                if (existingBonus != null)
                {
                    existingBonus.Bonus_Amount = dto.Bonus_Amount;
                    existingBonus.Incentive_Amount = dto.Incentive_Amount;
                }
                else
                {
                    var newBonus = new BonusIncentive
                    {
                        Payroll_Run_Id = dto.Payroll_Run_Id,
                        Employee_Id = dto.Employee_Id,
                        Bonus_Amount = dto.Bonus_Amount,
                        Incentive_Amount = dto.Incentive_Amount
                    };
                    _context.BonusIncentives.Add(newBonus);
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Bonus added/updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding bonus for payrollRunId {PayrollRunId}, employeeId {EmployeeId}", dto.Payroll_Run_Id, dto.Employee_Id);
                return BadRequest($"Error adding bonus: {ex.Message}");
            }
        }

        /// <summary>
        /// Finalizes a payroll run for all active employees.
        /// Iterates through employees, computes payroll, saves to EmployeePayrollRecord,
        /// generates PayoutSummary by PaymentMethod, and updates PayrollRun status.
        /// </summary>
        [HttpPost("finalize")]
        [Authorize(Roles = "Admin,SystemAdmin,HRAdmin")]
        public async Task<ActionResult> Finalize([FromBody] FinalizeRequestDto dto)
        {
            try
            {
                var payrollRun = await _context.PayrollRuns.FindAsync(dto.Payroll_Run_Id);
                if (payrollRun == null)
                {
                    return NotFound($"Payroll run with ID {dto.Payroll_Run_Id} not found.");
                }

                if (payrollRun.Status == "Finalized")
                {
                    return BadRequest("Payroll run is already finalized.");
                }

                // Fetch all active employees
                var activeEmployees = await _context.Employees
                    .Where(e => e.Status == "Active")
                    .ToListAsync<Employee>();

                if (!activeEmployees.Any())
                {
                    return BadRequest("No active employees found for payroll finalization.");
                }

                var payrollRecords = new List<EmployeePayrollRecord>();

                // Compute payroll for each employee
                foreach (var employee in activeEmployees)
                {
                    var computationResult = await _payrollComputationService.ComputePayrollAsync(dto.Payroll_Run_Id, employee.EmployeeId);

                    var payrollRecord = new EmployeePayrollRecord
                    {
                        Payroll_Run_Id = dto.Payroll_Run_Id,
                        Employee_Id = employee.EmployeeId,
                        Basic_Pay = computationResult.Basic_Pay,
                        OT_Pay = computationResult.OT_Pay,
                        Sss_Deduction = computationResult.Sss_Deduction,
                        PhilHealth_Deduction = computationResult.PhilHealth_Deduction,
                        PagIbig_Deduction = computationResult.PagIbig_Deduction,
                        Tax_Deduction = computationResult.Tax_Deduction,
                        Days_Worked = computationResult.Days_Worked,
                        OT_Hours = computationResult.OT_Hours,
                        Total_Deductions = computationResult.Total_Deductions,
                        Net_Pay = computationResult.Net_Pay
                    };

                    payrollRecords.Add(payrollRecord);

                    // Generate Payslip PDF for this employee
                    try
                    {
                        var pdfPath = await _payslipGeneratorService.GeneratePayslipAsync(
                            computationResult,
                            employee,
                            dto.Payroll_Run_Id,
                            DateOnly.FromDateTime(DateTime.UtcNow));

                        if (!string.IsNullOrEmpty(pdfPath))
                        {
                            var payslip = new Payslip
                            {
                                Payroll_Run_Id = dto.Payroll_Run_Id,
                                Employee_Id = employee.EmployeeId,
                                Payout_Date = DateOnly.FromDateTime(DateTime.UtcNow),
                                Pdf_Url = pdfPath,
                                Download_Url = $"/api/payroll/payslip/download/{dto.Payroll_Run_Id}/{employee.EmployeeId}",
                                Email_Sent_At = null,
                                Email_Retry_Count = 0
                            };

                            _context.Payslips.Add(payslip);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to generate payslip for EmployeeId {EmployeeId}", employee.EmployeeId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating payslip for EmployeeId {EmployeeId}", employee.EmployeeId);
                        // Continue to next employee (fault tolerance)
                    }
                }

                // Add all payroll records
                _context.EmployeePayrollRecords.AddRange(payrollRecords);

                // Group by PaymentMethod and generate PayoutSummary
                var payoutSummaries = payrollRecords
                    .GroupBy(pr => "Bank") // Default payment method since PaymentMethod was moved
                    .Select(g => new PayoutSummary
                    {
                        PayrollRunId = dto.Payroll_Run_Id,
                        PaymentMethod = g.Key,
                        TotalAmount = g.Sum(pr => pr.Net_Pay),
                        EmployeeCount = g.Count()
                    })
                    .ToList();

                _context.PayoutSummaries.AddRange(payoutSummaries);

                // Update PayrollRun
                payrollRun.Status = "Finalized";
                payrollRun.Finalized_At = DateTime.UtcNow;
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    payrollRun.Finalized_By = userId;
                }
                payrollRun.Payout_Summary_Json = JsonSerializer.Serialize(payoutSummaries.Select(ps => new
                {
                    ps.PayrollRunId,
                    ps.PaymentMethod,
                    ps.TotalAmount,
                    ps.EmployeeCount
                }));

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payroll finalized successfully and payslip PDFs generated.",
                    payrollRunId = dto.Payroll_Run_Id,
                    finalizedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizing payroll for payrollRunId {PayrollRunId}", dto.Payroll_Run_Id);
                return BadRequest($"Error finalizing payroll: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns a list of all payroll runs with employee details.
        /// Restricted to SystemAdmin and Manager roles.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,SystemAdmin,Manager,HR,HRAdmin")]
        public async Task<ActionResult<IEnumerable<PayrollRunListItemDto>>> GetPayrollRuns()
        {
            try
            {
                var payrollRuns = await _context.PayrollRuns
                    .Include(pr => pr.EmployeePayrollRecords)
                    .ThenInclude(epr => epr.Employee)
                    .ToListAsync();

                var result = payrollRuns.SelectMany(pr => pr.EmployeePayrollRecords.Select(epr => new PayrollRunListItemDto
                {
                    Id = epr.Id,
                    Employee_Id = epr.Employee_Id,
                    Employee_Name = $"{epr.Employee.FirstName} {epr.Employee.LastName}",
                    Position = epr.Employee.EmploymentDetails?.Position ?? "N/A",
                    Basic_Pay = epr.Basic_Pay,
                    OT_Pay = epr.OT_Pay,
                    Sss_Deduction = epr.Sss_Deduction,
                    PhilHealth_Deduction = epr.PhilHealth_Deduction,
                    PagIbig_Deduction = epr.PagIbig_Deduction,
                    Tax = epr.Tax_Deduction,
                    Bonus = 0, // TODO: Calculate from BonusIncentive
                    Net_Pay = epr.Net_Pay,
                    Status = pr.Status,
                    Payout_Method = "Bank" // Default payment method
                }));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payroll runs");
                return BadRequest($"Error retrieving payroll runs: {ex.Message}");
            }
        }

        /// <summary>
        /// Disburses payroll for a given payroll ID after payment file has been processed.
        /// Restricted to HRAdmin role.
        /// </summary>
        [HttpPut("{payrollId}/disburse")]
        [Authorize(Roles = "Admin,HRAdmin")]
        public async Task<ActionResult> Disburse(
            [FromRoute] int payrollId,
            [FromBody] DisbursePayrollDto dto)
        {
            try
            {
                var payrollRun = await _context.PayrollRuns.FindAsync(payrollId);
                if (payrollRun == null)
                {
                    return NotFound($"Payroll run with ID {payrollId} was not found.");
                }

                payrollRun.BatchReferenceNumber = dto.BatchReferenceNumber;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payroll disbursed successfully.", payrollId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disbursing payroll for payrollId {PayrollId}", payrollId);
                return BadRequest($"Error disbursing payroll: {ex.Message}");
            }
        }

        /// <summary>
        /// Downloads a payslip PDF for a specific employee and payroll run.
        /// Restricted to authenticated users.
        /// </summary>
        [HttpGet("payslip/download/{payrollRunId}/{employeeId}")]
        [Authorize]
        public async Task<IActionResult> DownloadPayslip(
            [FromRoute] int payrollRunId,
            [FromRoute] int employeeId)
        {
            try
            {
                var payslip = await _context.Payslips
                    .FirstOrDefaultAsync(p => p.Payroll_Run_Id == payrollRunId && p.Employee_Id == employeeId);

                if (payslip == null)
                {
                    return NotFound("Payslip not found.");
                }

                if (string.IsNullOrEmpty(payslip.Pdf_Url) || !System.IO.File.Exists(payslip.Pdf_Url))
                {
                    return NotFound("PDF file not found on server.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(payslip.Pdf_Url);
                var fileName = $"Payslip_{payrollRunId}_{employeeId}_{DateTime.UtcNow:yyyyMMdd}.pdf";

                return File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading payslip for payrollRunId {PayrollRunId}, employeeId {EmployeeId}", payrollRunId, employeeId);
                return StatusCode(500, "Error downloading payslip.");
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Payroll;
using Applications.Interfaces;

namespace Applications.Services
{
    public class PayrollComputationService : IPayrollComputationService
    {
        private readonly hrmAppDbContext _context;
        private readonly ILogger<PayrollComputationService> _logger;

        private const decimal OT_MULTIPLIER = 1.25m;
        private const decimal LUNCH_HOURS = 1.0m;
        private const decimal CUTOFFS_PER_YEAR = 24m;

        public PayrollComputationService(hrmAppDbContext context, ILogger<PayrollComputationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PayrollComputeResultDto> ComputePayrollAsync(int payrollRunId, int employeeId)
        {
            // Step 1: Fetch PayrollRun
            var payrollRun = await _context.PayrollRuns.FindAsync(payrollRunId);
            if (payrollRun == null)
            {
                throw new ArgumentException($"PayrollRun with ID {payrollRunId} not found.");
            }

            // Step 2: Fetch Employee Data with EmploymentDetails
            var employee = await _context.Employees
                .Include(e => e.EmploymentDetails)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {employeeId} not found.");
            }

            // Fetch active shift for the cutoff period
            var employeeShift = await _context.EmployeeShifts
                .Include(es => es.ShiftDefinition)
                .Where(es => es.Employee_Id == employeeId)
                .Where(es => es.Effective_Date <= payrollRun.CutOff_End_Date)
                .Where(es => es.End_Date == null || es.End_Date >= payrollRun.CutOff_Start_Date)
                .FirstOrDefaultAsync();

            var standardHours = employeeShift?.ShiftDefinition?.StandardHours ?? 8.0m;

            // Step 3: Query Attendance Logs
            var attendanceLogs = await _context.AttendanceLogs
                .Where(al => al.Employee_Id == employeeId)
                .Where(al => al.Log_Date >= payrollRun.CutOff_Start_Date)
                .Where(al => al.Log_Date <= payrollRun.CutOff_End_Date)
                .Where(al => al.Time_In.HasValue && al.Time_Out.HasValue)
                .ToListAsync();

            // Step 4: Calculate Days Worked and OT Hours
            decimal daysWorked = 0;
            decimal otHours = 0;

            foreach (var log in attendanceLogs)
            {
                var workedHours = (log.Time_Out.Value - log.Time_In.Value).TotalHours - (double)LUNCH_HOURS;
                if (workedHours < 0) workedHours = 0;

                if (workedHours >= (double)standardHours)
                {
                    daysWorked += 1;
                }
                else
                {
                    daysWorked += (decimal)workedHours / standardHours;
                }

                var excessHours = (decimal)workedHours - standardHours;
                if (excessHours >= 2m)
                {
                    otHours += excessHours;
                }
            }

            // Step 5: Compute Basic Pay
            var dailyRate = employee.EmploymentDetails?.BasePay / 22m ?? 0m; // Approximate daily rate from monthly base pay
            var basicPay = dailyRate * daysWorked;

            // Step 6: Compute OT Pay (using Hourly Rate)
            var hourlyRate = dailyRate / standardHours;
            var otPay = otHours * hourlyRate * OT_MULTIPLIER;

            // Step 7: Compute Gross Pay
            var grossPay = basicPay + otPay;

            // Step 8: Compute Statutory Deductions (using MonthlyBasePay for bracket lookup)
            var monthlyBasePay = employee.EmploymentDetails?.BasePay ?? 0m;
            var sssBracket = _context.SssBrackets
                .Where(b => b.EffectiveYear == 2024)
                .OrderByDescending(b => b.SalaryRangeStart)
                .FirstOrDefault(b => monthlyBasePay >= b.SalaryRangeStart);

            // SSS: Use only employee share (EmployeeShareRate is the employee's percentage)
            var sssDeduction = sssBracket != null 
                ? (monthlyBasePay * sssBracket.EmployeeShareRate) / 2m 
                : 0m;

            var philHealthBracket = _context.PhilHealthBrackets
                .Where(b => b.EffectiveYear == 2024)
                .OrderByDescending(b => b.SalaryRangeStart)
                .FirstOrDefault(b => monthlyBasePay >= b.SalaryRangeStart);

            // PhilHealth: Use only employee share (2.5% of basic pay, EmployeeShareRate = 0.025)
            var philHealthDeduction = philHealthBracket != null 
                ? (monthlyBasePay * philHealthBracket.EmployeeShareRate) / 2m 
                : 0m;

            var pagIbigBracket = _context.PagIbigBrackets
                .Where(b => b.EffectiveYear == 2024)
                .OrderByDescending(b => b.SalaryRangeStart)
                .FirstOrDefault(b => monthlyBasePay >= b.SalaryRangeStart);

            var pagIbigDeduction = pagIbigBracket != null 
                ? (monthlyBasePay * pagIbigBracket.EmployeeShareRate) / 2m 
                : 0m;

            // Step 9: Compute Taxable Income and Tax
            var taxableIncome = grossPay - (sssDeduction + philHealthDeduction + pagIbigDeduction);
            if (taxableIncome < 0) taxableIncome = 0;

            var annualTaxableIncome = taxableIncome * CUTOFFS_PER_YEAR;

            var taxBracket = _context.TaxBrackets
                .Where(b => b.EffectiveYear == 2024)
                .OrderByDescending(b => b.AnnualRangeStart)
                .FirstOrDefault(b => annualTaxableIncome >= b.AnnualRangeStart);

            decimal annualTax;
            if (taxBracket != null)
            {
                annualTax = taxBracket.BaseTax + ((annualTaxableIncome - taxBracket.AnnualRangeStart) * taxBracket.TaxRate);
            }
            else
            {
                annualTax = 0m;
            }

            var monthlyTax = annualTax / CUTOFFS_PER_YEAR;

            // Step 10: Compute Net Pay
            var totalDeductions = sssDeduction + philHealthDeduction + pagIbigDeduction + monthlyTax;
            var netPay = grossPay - totalDeductions;

            // Step 11: Build Result DTO
            return new PayrollComputeResultDto
            {
                Employee_Id = employeeId,
                Cutoff_Date = payrollRun.CutOff_End_Date,
                Payroll_Run_Id = payrollRunId,
                Daily_Rate = dailyRate,
                Days_Worked = (int)daysWorked,
                Basic_Pay = basicPay,
                OT_Hours = otHours,
                OT_Pay = otPay,
                Sss_Deduction = sssDeduction,
                PhilHealth_Deduction = philHealthDeduction,
                PagIbig_Deduction = pagIbigDeduction,
                Tax_Deduction = monthlyTax,
                Total_Deductions = totalDeductions,
                Net_Pay = netPay,
                Line_Items = new List<PayrollLineItemDto>()
            };
        }
    }
}

using Api.Contracts.Payroll;
using ApiHrm.Domains.Entities;

namespace Applications.Interfaces
{
    public interface IPayslipGeneratorService
    {
        Task<string> GeneratePayslipAsync(
            PayrollComputeResultDto computationResult,
            Employee employee,
            int payrollRunId,
            DateOnly payoutDate);
    }
}

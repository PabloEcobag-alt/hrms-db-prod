using Api.Contracts.Payroll;

namespace Applications.Interfaces
{
    public interface IPayrollComputationService
    {
        Task<PayrollComputeResultDto> ComputePayrollAsync(int payrollRunId, int employeeId);
    }
}

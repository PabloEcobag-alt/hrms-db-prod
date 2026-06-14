using Api.Contracts.Payroll;

namespace Applications.Interfaces
{
    public interface IPayrollService
    {
        Task<PayrollComputeResultDto> ComputeAsync(int employeeId, DateOnly cutoffDate);
        Task<PayrollRecordReadDto> FinalizeAsync(PayrollFinalizeRequestDto dto);
        Task<bool> DisbursePayrollAsync(int payrollId, DisbursePayrollDto dto, string disburserUserId, string disburserRole, string ipAddress);
        Task<IEnumerable<PayrollRunListItemDto>> GetPayrollRunsAsync();
    }
}

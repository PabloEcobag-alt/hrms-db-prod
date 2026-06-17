using Api.Contracts.CashAdvance;

namespace Applications.Interfaces
{
    public interface ICashAdvanceService
    {
        Task<CashAdvanceReadDto> CreateAsync(CashAdvanceCreateDto dto);
        Task<IEnumerable<CashAdvanceReadDto>> GetAllAsync();
    }
}

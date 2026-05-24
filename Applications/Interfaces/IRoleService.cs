using Api.Contracts.RoleExit;

namespace Applications.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleReadDto>> GetAllRolesAsync();
        Task<RoleReadDto> CreateRoleAsync(RoleCreateDto roleDto);
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.RoleExit;
using Applications.Interfaces;

namespace Applications.Services
{
    public class RoleService : IRoleService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        public RoleService(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleReadDto>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return _mapper.Map<IEnumerable<RoleReadDto>>(roles);
        }

        public async Task<RoleReadDto> CreateRoleAsync(RoleCreateDto roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return _mapper.Map<RoleReadDto>(role);
        }
    }
}
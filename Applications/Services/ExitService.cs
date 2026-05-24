using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Api.Contracts.RoleExit;
using Applications.Interfaces;

namespace Applications.Services
{
    public class ExitService : IExitService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        public ExitService(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExitReadDto>> GetAllExitsAsync()
        {
            var exits = await _context.Exits
                .Include(e => e.Employee) // To get the employee name for the DTO
                .ToListAsync();
            return _mapper.Map<IEnumerable<ExitReadDto>>(exits);
        }

        public async Task<ExitReadDto?> GetExitByIdAsync(int id)
        {
            var exit = await _context.Exits
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Exit_ID == id);
            return _mapper.Map<ExitReadDto>(exit);
        }

        public async Task<ExitReadDto> CreateExitRecordAsync(ExitCreateDto exitDto)
        {
            var exitEntity = _mapper.Map<Exit>(exitDto);

            _context.Exits.Add(exitEntity);

            var employee = await _context.Employees.FindAsync(exitDto.Employee_ID);
            if (employee != null)
            {
                employee.Status = "Inactive";
            }

            await _context.SaveChangesAsync();

            var result = await _context.Exits
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Exit_ID == exitEntity.Exit_ID);

            return _mapper.Map<ExitReadDto>(result);
        }
        public async Task<bool> UpdateExitRecordAsync(int id, ExitCreateDto updateDto)
        {
            var exit = await _context.Exits.FindAsync(id);
            if (exit == null) return false;

            _mapper.Map(updateDto, exit);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
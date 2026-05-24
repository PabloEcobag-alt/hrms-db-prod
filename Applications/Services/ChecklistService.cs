using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Checklist;
using Applications.Interfaces;

namespace Applications.Services
{
    public class ChecklistService : IChecklistService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        public ChecklistService(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ChecklistReadDto?> GetChecklistByApplicantIdAsync(int applicantId)
        {
            var checklist = await _context.Checklists
                .FirstOrDefaultAsync(c => c.Applicant_ID == applicantId);
            return _mapper.Map<ChecklistReadDto>(checklist);
        }

        public async Task<bool> UpdateChecklistAsync(ChecklistUpdateDto updateDto)
        {
            var checklist = await _context.Checklists.FindAsync(updateDto.Checklist_ID);
            if (checklist == null) return false;

            _mapper.Map(updateDto, checklist);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
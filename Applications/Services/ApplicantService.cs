using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Applicant;
using Api.Contracts.Employee;
using Api.Contracts.Checklist;
using Applications.Interfaces;

namespace Applications.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        public ApplicantService(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ChecklistReadDto> GetChecklistByApplicantAsync(int applicantId)
        {
            var checklist = await _context.Checklists
                .FirstOrDefaultAsync(c => c.Applicant_ID == applicantId);

            // If you return 'checklist' directly, check if the DTO has Applicant_ID
            return _mapper.Map<ChecklistReadDto>(checklist);
        }

        public async Task<IEnumerable<ApplicantReadDto>> GetAllApplicantsAsync()
        {
            var applicants = await _context.Applicants.ToListAsync();
            return _mapper.Map<IEnumerable<ApplicantReadDto>>(applicants);
        }

        public async Task<ApplicantDetailDto?> GetApplicantByIdAsync(int id)
        {
            var applicant = await _context.Applicants
                .Include(a => a.Checklists)
                .Include(a => a.Documents)
                .FirstOrDefaultAsync(a => a.Applicant_ID == id);

            return _mapper.Map<ApplicantDetailDto>(applicant);
        }

        public async Task<bool> UpdateChecklistAsync(int applicantId, ChecklistUpdateDto dto)
        {
            var checklist = await _context.Checklists
                .FirstOrDefaultAsync(c => c.Applicant_ID == applicantId);

            if (checklist == null) return false;

            checklist.Has_NBI = dto.Has_NBI ?? checklist.Has_NBI;
            checklist.Has_Medical = dto.Has_Medical ?? checklist.Has_Medical;
            checklist.Has_Xray = dto.Has_Xray ?? checklist.Has_Xray;
            checklist.has_SSS = dto.has_SSS ?? checklist.has_SSS;
            checklist.has_PAGIBIG = dto.has_PAGIBIG ?? checklist.has_PAGIBIG;
            checklist.has_PhilHealth = dto.has_PhilHealth ?? checklist.has_PhilHealth;
            checklist.has_TIN = dto.has_TIN ?? checklist.has_TIN;

            try 
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                //testing
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<ApplicantReadDto> CreateApplicantAsync(ApplicantCreateDto createDto)
        {
            var applicant = _mapper.Map<Applicant>(createDto);

            applicant.Application_Date = DateTime.UtcNow;
            
            if (applicant.Checklists == null)
                {
                    applicant.Checklists = new List<Checklist>();
                }

            _context.Applicants.Add(applicant);
            
            // Initialize the checklist for Philippine compliance (NBI, Medical, X-ray)
            applicant.Checklists.Add(new Checklist { 
                Has_NBI = false, 
                Has_Medical = false, 
                Has_Xray = false,
                has_SSS = false,
                has_PAGIBIG = false,
                has_PhilHealth = false,
                has_TIN = false 
            });

            await _context.SaveChangesAsync();

            var result = await _context.Applicants
            .FirstOrDefaultAsync(a => a.Applicant_ID == applicant.Applicant_ID);

            return _mapper.Map<ApplicantReadDto>(applicant);
        }

        public async Task<bool> UpdateApplicantStatusAsync(int id, string status)
        {
            var applicant = await _context.Applicants.FindAsync(id);
            if (applicant == null) return false;

            applicant.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<EmployeeReadDto?> HireApplicantAsync(int applicantId, int roleId)
        {
            var applicant = await _context.Applicants.FindAsync(applicantId);
            if (applicant == null) return null;

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) throw new Exception("Role not found");

            var newEmployee = new Employee
            {
                First_Name = applicant.First_Name,
                Last_Name = applicant.Last_Name,
                Email = applicant.Email,
                Position = role.Role_Description,
                Role_ID = roleId,
                Contact_Details = applicant.Contact_Details,
                Payment_Method = applicant.Payment_Method,
                Status = "Active",
                Hire_Date = DateTime.UtcNow 
            };

            _context.Employees.Add(newEmployee);
            applicant.Status = "Hired"; 
            
            await _context.SaveChangesAsync();

            await _context.Entry(newEmployee).Reference(e => e.role).LoadAsync();

            var result = await _context.Employees
            .Include(e => e.role)
            .FirstOrDefaultAsync(e => e.Employee_Id == newEmployee.Employee_Id);

            return _mapper.Map<EmployeeReadDto>(newEmployee);
        }
    }
}
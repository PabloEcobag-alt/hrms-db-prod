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
            try
            {
                var applicants = await _context.Applicants.ToListAsync();
                return _mapper.Map<IEnumerable<ApplicantReadDto>>(applicants);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllApplicantsAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
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

            applicant.Application_Date = DateOnly.FromDateTime(DateTime.UtcNow);
            
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
                FirstName = applicant.First_Name,
                LastName = applicant.Last_Name,
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25)), // Default value
                Gender = "Not Specified", // Default value
                CivilStatus = "Single", // Default value
                BloodType = "O+", // Default value
                Status = applicant.Status ?? "Active", // Use applicant's status (Training/Probationary/Regular)
                Role_ID = roleId,
                AvatarIndex = 0,
                ErpUserId = null // Keep null initially, will be set during registration
            };

            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            // Create ContactInformation with phone number from applicant
            var contactInfo = new ContactInformation
            {
                EmployeeId = newEmployee.EmployeeId,
                EmailAddress = applicant.Email,
                PhoneNumber = applicant.Phone,
                PresentAddress = "Not specified",
                PermanentAddress = "Not specified"
            };
            _context.ContactInformation.Add(contactInfo);

            // Create EmploymentDetails with position from applicant
            var employmentDetails = new EmploymentDetails
            {
                EmployeeId = newEmployee.EmployeeId,
                Position = applicant.Position,
                Department = "General", // Default department
                HireDate = applicant.Expected_Start_Date ?? DateOnly.FromDateTime(DateTime.UtcNow),
                EmploymentStatus = applicant.Status ?? "Regular", // Use applicant's status (Training/Probationary/Regular)
                BasePay = 0
            };
            _context.EmploymentDetails.Add(employmentDetails);

            applicant.Status = "Hired"; 
            
            await _context.SaveChangesAsync();

            await _context.Entry(newEmployee).Reference(e => e.role).LoadAsync();

            var result = await _context.Employees
            .Include(e => e.role)
            .FirstOrDefaultAsync(e => e.EmployeeId == newEmployee.EmployeeId);

            return _mapper.Map<EmployeeReadDto>(newEmployee);
        }

        public async Task<ApplicantReadDto> CreateEcommerceApplicationAsync(EcommerceApplicationDto dto)
        {
            var applicant = new Applicant
            {
                First_Name = dto.FirstName,
                Last_Name = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Position = dto.Position,
                Source = "Website Portal",
                Hiring_Stage = "Initial Interview",
                Status = "Pending",
                Application_Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Interview_Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), // Auto-set 2 days from application
                Expected_Start_Date = null,
                Resume_URL = dto.ResumeFileName ?? "",
                Contact_Details = dto.CoverLetter,
                Payment_Method = null,
                NBI_Document_Completed = false,
                Medical_Document_Completed = false,
                XRay_Document_Completed = false
            };

            if (applicant.Checklists == null)
            {
                applicant.Checklists = new List<Checklist>();
            }

            _context.Applicants.Add(applicant);

            // Initialize the checklist for Philippine compliance
            applicant.Checklists.Add(new Checklist
            {
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

        public async Task<ApplicantReadDto?> GetRecentEcommerceApplicantAsync()
        {
            var applicant = await _context.Applicants
                .Where(a => a.Source == "Website Portal")
                .OrderByDescending(a => a.Application_Date)
                .ThenByDescending(a => a.Applicant_ID)
                .FirstOrDefaultAsync();

            if (applicant == null) return null;

            return _mapper.Map<ApplicantReadDto>(applicant);
        }

        public async Task<ApplicantReadDto?> UpdateApplicantAsync(int id, ApplicantUpdateDto updateDto)
        {
            var applicant = await _context.Applicants.FindAsync(id);
            
            if (applicant == null) return null;

            if (updateDto.Hiring_Stage != null)
                applicant.Hiring_Stage = updateDto.Hiring_Stage;
            
            if (updateDto.Interview_Date != null)
                applicant.Interview_Date = updateDto.Interview_Date;
            
            if (updateDto.Expected_Start_Date != null)
                applicant.Expected_Start_Date = updateDto.Expected_Start_Date;
            
            if (updateDto.Status != null)
                applicant.Status = updateDto.Status;
            
            if (updateDto.Position != null)
                applicant.Position = updateDto.Position;
            
            if (updateDto.Phone != null)
                applicant.Phone = updateDto.Phone;
            
            if (updateDto.Email != null)
                applicant.Email = updateDto.Email;

            await _context.SaveChangesAsync();

            var result = await _context.Applicants
                .FirstOrDefaultAsync(a => a.Applicant_ID == id);

            return _mapper.Map<ApplicantReadDto>(result);
        }
    }
}
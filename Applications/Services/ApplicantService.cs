using System.Threading.Channels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Applicant;
using Api.Contracts.Employee;
using Api.Contracts.Checklist;
using Applications.Interfaces;
using Applications.Commands;
using PhoneNumbers;

namespace Applications.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly Channel<int> _scoringQueue;

        public ApplicantService(hrmAppDbContext context, IMapper mapper, Channel<int> scoringQueue)
        {
            _context = context;
            _mapper = mapper;
            _scoringQueue = scoringQueue;
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
                var applicants = await _context.Applicants.Include(a => a.Checklists).ToListAsync();
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
            // Sanitize position field to prevent string mismatches
            if (!string.IsNullOrEmpty(createDto.position))
            {
                createDto.position = createDto.position.Replace("Intern / Summer Job", "Intern/Summer Job").Trim();
            }

            var applicant = new Applicant
            {
                First_Name = createDto.firstName,
                Middle_Name = createDto.middleName,
                Last_Name = createDto.lastName,
                Email = createDto.email,
                Mobile = createDto.mobile,
                Position = createDto.position,
                Source = createDto.source,
                Hiring_Stage = createDto.hiringStage ?? "Initial Interview",
                Interview_Date = createDto.interviewDate,
                Expected_Start_Date = createDto.expectedStart,
                Probationary_End_Date = createDto.probationaryEndDate,
                Contact_Details = createDto.Contact_Details,
                Payment_Method = createDto.Payment_Method,
                Resume_URL = createDto.Resume_URL,
                Status = createDto.Status ?? "Training",
                Application_Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            
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

            // Fire-and-forget: enqueue for AI scoring so the dashboard can show a match score.
            _scoringQueue.Writer.TryWrite(applicant.Applicant_ID);

            var result = await _context.Applicants
            .Include(a => a.Checklists)
            .FirstOrDefaultAsync(a => a.Applicant_ID == applicant.Applicant_ID);

            return _mapper.Map<ApplicantReadDto>(result);
        }

        public async Task<bool> UpdateApplicantStatusAsync(int id, string status)
        {
            var applicant = await _context.Applicants.FindAsync(id);
            if (applicant == null) return false;

            applicant.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<EmployeeReadDto?> HireApplicantAsync(HireApplicantDto dto)
        {
            var applicant = await _context.Applicants.FindAsync(dto.ApplicantId);
            if (applicant == null) return null;

            // Use default role ID 1 for now (can be configured later)
            var roleId = 1;
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) throw new Exception("Role not found");

            var newEmployee = new Employee
            {
                FirstName = applicant.First_Name,
                MiddleName = applicant.Middle_Name,
                LastName = applicant.Last_Name,
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25)), // Default value
                Gender = "Not Specified", // Default value
                CivilStatus = "Single", // Default value
                BloodType = "O+", // Default value
                Status = "Active", // Set to Active for new hires
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
                PhoneNumber = applicant.Mobile ?? applicant.Contact_Details,
                PresentAddress = "Not specified",
                PermanentAddress = "Not specified"
            };
            _context.ContactInformation.Add(contactInfo);

            // Create EmploymentDetails with position from applicant and new hire dates
            var employmentDetails = new EmploymentDetails
            {
                EmployeeId = newEmployee.EmployeeId,
                Position = applicant.Position,
                Department = "General", // Default department
                HireDate = DateOnly.FromDateTime(dto.StartDate),
                EmploymentStatus = dto.HiringStage, // Use the hiring stage from payload
                ProbationaryEndDate = dto.ProbationaryEndDate.HasValue 
                    ? DateOnly.FromDateTime(dto.ProbationaryEndDate.Value) 
                    : null,
                BasePay = 0
            };
            _context.EmploymentDetails.Add(employmentDetails);

            // Update applicant's hiring stage to match payload
            applicant.Hiring_Stage = dto.HiringStage;
            applicant.Expected_Start_Date = DateOnly.FromDateTime(dto.StartDate);
            if (dto.ProbationaryEndDate.HasValue)
            {
                applicant.Probationary_End_Date = DateOnly.FromDateTime(dto.ProbationaryEndDate.Value);
            }
            
            await _context.SaveChangesAsync();

            await _context.Entry(newEmployee).Reference(e => e.role).LoadAsync();

            var result = await _context.Employees
            .Include(e => e.role)
            .FirstOrDefaultAsync(e => e.EmployeeId == newEmployee.EmployeeId);

            return _mapper.Map<EmployeeReadDto>(newEmployee);
        }

        public async Task<ApplicantReadDto> CreateEcommerceApplicationAsync(EcommerceApplicationDto dto)
        {
            string resumeUrl = "";
            if (dto.ResumeFile != null)
            {
                // Generate unique filename
                var fileExtension = Path.GetExtension(dto.ResumeFile.FileName);
                var uniqueFileName = $"resume_{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "resumes");

                // Ensure directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ResumeFile.CopyToAsync(stream);
                }

                resumeUrl = $"/uploads/resumes/{uniqueFileName}";
            }

            // LAYER 2: Application-Level Security (Sanitization & Deduplication)
            // 1. Token Protection: Limit CoverLetter to prevent large payloads draining OpenAI tokens
            if (!string.IsNullOrEmpty(dto.CoverLetter) && dto.CoverLetter.Length > 2000)
            {
                dto.CoverLetter = dto.CoverLetter.Substring(0, 2000);
            }

            // 2. Deduplication: Prevent spam from same Email/Mobile in the last 6 months
            var sixMonthsAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6));
            var recentDuplicate = await _context.Applicants
                .Where(a => a.Application_Date >= sixMonthsAgo && 
                            (a.Email == dto.Email || a.Mobile == dto.Mobile))
                .FirstOrDefaultAsync();

            if (recentDuplicate != null)
            {
                throw new InvalidOperationException("You have already submitted an application recently. Please wait before reapplying.");
            }

            // Sanitize position field to prevent string mismatches
            if (!string.IsNullOrEmpty(dto.Position))
            {
                dto.Position = dto.Position.Replace("Intern / Summer Job", "Intern/Summer Job").Trim();
            }

            var applicant = new Applicant
            {
                First_Name = dto.FirstName,
                Middle_Name = dto.MiddleName,
                Last_Name = dto.LastName,
                Email = dto.Email,
                Mobile = dto.Mobile,
                Position = dto.Position,
                Source = "Website Portal",
                Hiring_Stage = "Initial Interview",
                Status = "Pending",
                Application_Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Interview_Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), // Auto-set 2 days from application
                Expected_Start_Date = null,
                Resume_URL = resumeUrl,
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

            // Fire-and-forget: enqueue for AI scoring so the frontend request
            // returns instantly without waiting for the OpenAI response.
            _scoringQueue.Writer.TryWrite(applicant.Applicant_ID);

            var result = await _context.Applicants
                .Include(a => a.Checklists)
                .FirstOrDefaultAsync(a => a.Applicant_ID == applicant.Applicant_ID);

            return _mapper.Map<ApplicantReadDto>(result);
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
            var applicant = await _context.Applicants
                .Include(a => a.Checklists)
                .FirstOrDefaultAsync(a => a.Applicant_ID == id);
            
            if (applicant == null) return null;

            if (updateDto.First_Name != null)
                applicant.First_Name = updateDto.First_Name;
            
            if (updateDto.Middle_Name != null)
                applicant.Middle_Name = updateDto.Middle_Name;
            
            if (updateDto.Last_Name != null)
                applicant.Last_Name = updateDto.Last_Name;
            
            if (updateDto.Hiring_Stage != null)
                applicant.Hiring_Stage = updateDto.Hiring_Stage;
            
            if (updateDto.Interview_Date != null)
                applicant.Interview_Date = updateDto.Interview_Date;
            
            if (updateDto.Expected_Start_Date != null)
                applicant.Expected_Start_Date = updateDto.Expected_Start_Date;
            
            if (updateDto.Probationary_End_Date != null)
                applicant.Probationary_End_Date = updateDto.Probationary_End_Date;
            
            if (updateDto.Status != null)
                applicant.Status = updateDto.Status;
            
            if (updateDto.Position != null)
                applicant.Position = updateDto.Position;
            
            if (updateDto.Mobile != null)
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();
                try
                {
                    var parsedNumber = phoneUtil.Parse(updateDto.Mobile, "PH");
                    if (!phoneUtil.IsValidNumber(parsedNumber))
                    {
                        throw new ArgumentException("Invalid phone number format.");
                    }
                    applicant.Mobile = updateDto.Mobile;
                }
                catch
                {
                    throw new ArgumentException("Invalid phone number format.");
                }
            }
            
            if (updateDto.Email != null)
                applicant.Email = updateDto.Email;
            
            if (updateDto.Interview_Notes != null)
                applicant.Interview_Notes = updateDto.Interview_Notes;

            if (updateDto.Requirements != null)
            {
                var checklist = applicant.Checklists?.FirstOrDefault();
                if (checklist == null)
                {
                    checklist = new Checklist { Applicant_ID = applicant.Applicant_ID };
                    if (applicant.Checklists == null) applicant.Checklists = new List<Checklist>();
                    applicant.Checklists.Add(checklist);
                }

                if (updateDto.Requirements.TryGetValue("nbi", out bool nbi)) checklist.Has_NBI = nbi;
                if (updateDto.Requirements.TryGetValue("medical", out bool medical)) checklist.Has_Medical = medical;
                if (updateDto.Requirements.TryGetValue("xray", out bool xray)) checklist.Has_Xray = xray;
                if (updateDto.Requirements.TryGetValue("sss", out bool sss)) checklist.has_SSS = sss;
                if (updateDto.Requirements.TryGetValue("pagibig", out bool pagibig)) checklist.has_PAGIBIG = pagibig;
                if (updateDto.Requirements.TryGetValue("philhealth", out bool philhealth)) checklist.has_PhilHealth = philhealth;
                if (updateDto.Requirements.TryGetValue("tin", out bool tin)) checklist.has_TIN = tin;
            }

            await _context.SaveChangesAsync();

            var result = await _context.Applicants
                .Include(a => a.Checklists)
                .FirstOrDefaultAsync(a => a.Applicant_ID == id);

            return _mapper.Map<ApplicantReadDto>(result);
        }
    }
}
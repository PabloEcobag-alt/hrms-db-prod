using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Digital201;
using Applications.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Applications.Services
{
    public class Digital201Service : IDigital201Service
    {
        private readonly hrmAppDbContext _context;
        private readonly ILogger<Digital201Service> _logger;

        public Digital201Service(hrmAppDbContext context, ILogger<Digital201Service> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Generate ErpUserId for now (until Auth integration)
                var erpUserId = Guid.NewGuid().ToString();
                
                // Step 1: Create Employee Record
                var employee = new Employee
                {
                    ErpUserId = erpUserId,
                    FirstName = dto.FirstName,
                    MiddleName = dto.MiddleName,
                    LastName = dto.LastName,
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-25)), // Default value
                    Gender = "Not Specified",
                    CivilStatus = "Not Specified",
                    Status = "Regular", // Default status
                    AvatarIndex = 0,
                    BloodType = "",
                    Role_ID = 1
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                var employeeId = employee.EmployeeId;

                // Step 2: Create Contact Information
                var contactInfo = new ContactInformation
                {
                    EmployeeId = employeeId,
                    EmailAddress = dto.Email,
                    PhoneNumber = dto.ContactNo,
                    PresentAddress = "Not specified",
                    PermanentAddress = "Not specified"
                };

                _context.ContactInformation.Add(contactInfo);
                await _context.SaveChangesAsync();

                // Step 3: Create Employment Details
                var employmentDetails = new EmploymentDetails
                {
                    EmployeeId = employeeId,
                    HireDate = DateOnly.FromDateTime(dto.DateHired),
                    Tenure = "0 years",
                    EmploymentStatus = "Regular",
                    Department = "General", // Default department
                    Position = dto.Position,
                    SalaryGrade = "1",
                    BasePay = 0 // Default value
                };

                _context.EmploymentDetails.Add(employmentDetails);
                await _context.SaveChangesAsync();

                // Step 4: Create Emergency Contact if provided
                if (!string.IsNullOrEmpty(dto.EmergencyContactName))
                {
                    var emergencyContact = new EmergencyContact
                    {
                        EmployeeId = employeeId,
                        FirstName = dto.EmergencyContactName.Split(' ').FirstOrDefault() ?? dto.EmergencyContactName,
                        LastName = dto.EmergencyContactName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                        PhoneNumber = dto.EmergencyContactPhone,
                        Address = "Not specified",
                        Relationship = dto.EmergencyContactRelationship
                    };

                    _context.EmergencyContacts.Add(emergencyContact);
                    await _context.SaveChangesAsync();
                }

                // Step 5: Handle Document Uploads
                await ProcessDocumentUploads(employeeId, dto);

                await transaction.CommitAsync();
                
                _logger.LogInformation("Successfully created employee with ID: {EmployeeId}", employeeId);
                return employeeId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating employee");
                throw;
            }
        }

        private async Task ProcessDocumentUploads(int employeeId, CreateEmployeeDto dto)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads", "employees", employeeId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            // Step 1: Initial Documents
            await SaveDocumentFile(employeeId, dto.ResumeFile, "Resume", "Personal");
            await SaveDocumentFile(employeeId, dto.PersonalDataSheetFile, "Personal Data Sheet", "Personal");
            await SaveDocumentFile(employeeId, dto.IdPictureFile, "2x2 ID Picture", "Personal");
            await SaveDocumentFile(employeeId, dto.BirthCertificateFile, "Birth Certificate", "Personal");
            await SaveDocumentFile(employeeId, dto.MarriageCertificateFile, "Marriage Certificate", "Personal");

            // Step 3: Employment Documents
            await SaveDocumentFile(employeeId, dto.JobDescriptionFile, "Job Description", "Employment");
            await SaveDocumentFile(employeeId, dto.EmploymentContractFile, "Employment Contract", "Employment");
            await SaveDocumentFile(employeeId, dto.CompanyRulesFile, "Company Rules & Regs", "Employment");
            await SaveDocumentFile(employeeId, dto.NDAFile, "NDA", "Employment");
            await SaveDocumentFile(employeeId, dto.HandbookFile, "Handbook Acknowledgment", "Employment");

            // Step 3: Payroll Documents
            await SaveDocumentFile(employeeId, dto.SalaryAgreementFile, "Salary Agreement", "Payroll");
            await SaveDocumentFile(employeeId, dto.BIR2316File, "BIR 2316", "Payroll");
            await SaveDocumentFile(employeeId, dto.AttendanceRecordFile, "Attendance Record", "Payroll");

            // Step 3: Property Documents
            await SaveDocumentFile(employeeId, dto.AcknowledgmentReceiptFile, "Acknowledgment Receipt", "Property");

            // Step 4: Health Documents
            await SaveDocumentFile(employeeId, dto.MedicalCertificateFile, "Medical Certificate", "Health");
            await SaveDocumentFile(employeeId, dto.DrugTestFile, "Drug Test Result", "Health");
            await SaveDocumentFile(employeeId, dto.VaccinationCardFile, "Vaccination Card", "Health");

            // Step 4: Performance Documents
            await SaveDocumentFile(employeeId, dto.PerformanceEvaluationFile, "Performance Evaluation", "Performance");
            await SaveDocumentFile(employeeId, dto.IncidentReportFile, "Incident Report", "Performance");
            await SaveDocumentFile(employeeId, dto.DisciplinaryRecordFile, "Disciplinary Record", "Performance");
            await SaveDocumentFile(employeeId, dto.PromotionRecordFile, "Promotion Record", "Performance");
        }

        private async Task SaveDocumentFile(int employeeId, IFormFile? file, string documentName, string category)
        {
            if (file == null || file.Length == 0) return;

            try
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "employees", employeeId.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{documentName.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = Path.Combine("uploads", "employees", employeeId.ToString(), fileName).Replace("\\", "/");

                var document = new EmployeeDocument
                {
                    EmployeeId = employeeId,
                    DocumentName = documentName,
                    DocumentType = category,
                    FileUrl = relativePath,
                    UploadDate = DateTime.UtcNow,
                    Status = "Uploaded",
                    VerificationStatus = "Pending_Verification",
                    Issue_Date = DateOnly.FromDateTime(DateTime.Now),
                    Expiry_Date = category == "Government" && documentName.Contains("Clearance") 
                        ? DateOnly.FromDateTime(DateTime.Now.AddYears(1)) 
                        : DateOnly.FromDateTime(DateTime.Now.AddYears(5))
                };

                _context.EmployeeDocuments.Add(document);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Saved document: {DocumentName} for employee: {EmployeeId}", documentName, employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving document: {DocumentName} for employee: {EmployeeId}", documentName, employeeId);
                throw;
            }
        }

        public async Task<List<AdminEmployeeListDto>> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await _context.Employees
                    .Include(e => e.EmploymentDetails)
                    .Include(e => e.ContactInformation)
                    .Select(e => new AdminEmployeeListDto
                    {
                        EmployeeId = e.EmployeeId,
                        ErpUserId = e.ErpUserId,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Department = e.EmploymentDetails != null ? e.EmploymentDetails.Department : string.Empty,
                        Position = e.EmploymentDetails != null ? e.EmploymentDetails.Position : string.Empty,
                        Status = e.Status,
                        EmailAddress = e.ContactInformation != null ? e.ContactInformation.EmailAddress : string.Empty
                    })
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToListAsync();

                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all employees");
                return new List<AdminEmployeeListDto>();
            }
        }

        public async Task<GetContactInfoResponseDto?> GetContactInfoAsync(string erpUserId)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.ContactInformation)
                    .Include(e => e.EmergencyContacts)
                    .FirstOrDefaultAsync(e => e.ErpUserId == erpUserId);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found for ErpUserId: {ErpUserId}", erpUserId);
                    return null;
                }

                var response = new GetContactInfoResponseDto
                {
                    Email = employee.ContactInformation?.EmailAddress ?? string.Empty,
                    Phone = employee.ContactInformation?.PhoneNumber ?? string.Empty,
                    Address = employee.ContactInformation?.PresentAddress ?? string.Empty,
                    EmergencyContact = employee.EmergencyContacts?.FirstOrDefault() != null
                        ? new EmergencyContactDto
                        {
                            FirstName = employee.EmergencyContacts.First().FirstName,
                            LastName = employee.EmergencyContacts.First().LastName,
                            Relationship = employee.EmergencyContacts.First().Relationship,
                            Phone = employee.EmergencyContacts.First().PhoneNumber,
                            Address = employee.EmergencyContacts.First().Address
                        }
                        : new EmergencyContactDto()
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact info for ErpUserId: {ErpUserId}", erpUserId);
                return null;
            }
        }

        public async Task<bool> UpdateContactInfoAsync(string erpUserId, UpdateContactInfoRequestDto request)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Find employee by ErpUserId
                var employee = await _context.Employees
                    .Include(e => e.ContactInformation)
                    .Include(e => e.EmergencyContacts)
                    .FirstOrDefaultAsync(e => e.ErpUserId == erpUserId);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found for ErpUserId: {ErpUserId}", erpUserId);
                    return false;
                }

                // Update or create ContactInformation
                if (employee.ContactInformation == null)
                {
                    employee.ContactInformation = new ContactInformation
                    {
                        EmployeeId = employee.EmployeeId,
                        EmailAddress = request.Email,
                        PhoneNumber = request.Phone,
                        PresentAddress = request.Address
                    };
                    // ContactInformation will be automatically tracked via navigation property
                }
                else
                {
                    employee.ContactInformation.EmailAddress = request.Email;
                    employee.ContactInformation.PhoneNumber = request.Phone;
                    employee.ContactInformation.PresentAddress = request.Address;
                }

                // Replace strategy: Delete all existing emergency contacts
                var existingEmergencyContacts = await _context.EmergencyContacts
                    .Where(ec => ec.EmployeeId == employee.EmployeeId)
                    .ToListAsync();

                _context.EmergencyContacts.RemoveRange(existingEmergencyContacts);

                // Add new emergency contact
                var newEmergencyContact = new EmergencyContact
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = request.EmergencyContact.FirstName,
                    LastName = request.EmergencyContact.LastName,
                    Relationship = request.EmergencyContact.Relationship,
                    PhoneNumber = request.EmergencyContact.Phone,
                    Address = request.EmergencyContact.Address
                };

                _context.EmergencyContacts.Add(newEmergencyContact);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully updated contact info for employee {EmployeeId}", employee.EmployeeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact info for ErpUserId: {ErpUserId}", erpUserId);
                return false;
            }
        }
    }
}

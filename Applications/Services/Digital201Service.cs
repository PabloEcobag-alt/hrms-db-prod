using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Digital201;
using Applications.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Http;

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
            // Check for duplicate employee with same first and last name
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.FirstName.ToLower() == dto.FirstName.ToLower() && 
                                         e.LastName.ToLower() == dto.LastName.ToLower());
            
            if (existingEmployee != null)
            {
                throw new BadHttpRequestException("An employee with this exact first and last name already exists.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Step 1: Create Employee Record
                // ErpUserId is left null - will be assigned by Auth service during registration
                var employee = new Employee
                {
                    ErpUserId = null,
                    FirstName = dto.FirstName,
                    MiddleName = dto.MiddleName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth,
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

                // Step 5: Create Government ID if any government IDs are provided
                if (!string.IsNullOrEmpty(dto.SSS) || !string.IsNullOrEmpty(dto.PhilHealth) || 
                    !string.IsNullOrEmpty(dto.PagIbig) || !string.IsNullOrEmpty(dto.TIN))
                {
                    var governmentId = new GovernmentId
                    {
                        Employee_Id = employeeId,
                        SSS_Number = dto.SSS ?? "",
                        PhilHealth_Number = dto.PhilHealth ?? "",
                        HDMF_Number = dto.PagIbig ?? "",
                        TIN_Number = dto.TIN ?? ""
                    };

                    _context.GovernmentIds.Add(governmentId);
                    await _context.SaveChangesAsync();
                }

                // Step 6: Handle Document Uploads
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
                _logger.LogInformation("Starting GetAllEmployeesAsync query");
                
                var employees = await _context.Employees
                    .Include(e => e.EmploymentDetails)
                    .Include(e => e.ContactInformation)
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToListAsync();

                var result = employees.Select(e => new AdminEmployeeListDto
                {
                    EmployeeId = e.EmployeeId,
                    ErpUserId = e.ErpUserId ?? string.Empty,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Department = e.EmploymentDetails?.Department ?? string.Empty,
                    Position = e.EmploymentDetails?.Position ?? string.Empty,
                    Status = e.Status,
                    EmailAddress = e.ContactInformation?.EmailAddress ?? string.Empty
                }).ToList();

                _logger.LogInformation("Retrieved {Count} employees from database", result.Count);
                
                return result;
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
                _logger.LogInformation("Looking up employee for ErpUserId: {ErpUserId}", erpUserId);
                
                // Log all employees with their ErpUserIds for debugging
                var allEmployees = await _context.Employees
                    .Select(e => new { e.EmployeeId, e.FirstName, e.LastName, e.ErpUserId })
                    .ToListAsync();
                
                _logger.LogInformation("Found {Count} employees in database", allEmployees.Count);
                foreach (var emp in allEmployees.Take(5))
                {
                    _logger.LogInformation("Employee: {EmployeeId} - {FirstName} {LastName}, ErpUserId: {ErpUserId}", 
                        emp.EmployeeId, emp.FirstName, emp.LastName, emp.ErpUserId ?? "NULL");
                }

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

        public async Task<bool> CheckDuplicateEmployeeAsync(string firstName, string lastName)
        {
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.FirstName.ToLower() == firstName.ToLower() && 
                                         e.LastName.ToLower() == lastName.ToLower());
            
            return existingEmployee != null;
        }

        public async Task<List<ExpiringDocumentDto>> GetExpiringDocumentsAsync(int days)
        {
            var cutoffDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(days));
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            
            var expiringDocuments = await _context.EmployeeDocuments
                .Where(ed => ed.Expiry_Date <= cutoffDate && ed.Expiry_Date > today)
                .Select(ed => new ExpiringDocumentDto
                {
                    EmployeeId = ed.EmployeeId,
                    EmployeeName = ed.Employee.FirstName + " " + ed.Employee.LastName,
                    DocumentType = ed.DocumentType,
                    DocumentName = ed.DocumentName,
                    ExpirationDate = ed.Expiry_Date.ToDateTime(TimeOnly.MinValue),
                    DaysUntilExpiration = ed.Expiry_Date.DayNumber - today.DayNumber
                })
                .OrderBy(ed => ed.ExpirationDate)
                .ToListAsync();

            return expiringDocuments;
        }

        public async Task<EmployeeProfileDto?> GetEmployeeProfileAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.EmploymentDetails)
                .Include(e => e.ContactInformation)
                .Include(e => e.GovernmentId)
                .Include(e => e.EmergencyContacts)
                .Include(e => e.CompanyProperty)
                .Include(e => e.role)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
                return null;

            // Initialize missing entities to prevent null reference exceptions
            if (employee.EmploymentDetails == null)
                employee.EmploymentDetails = new EmploymentDetails();

            if (employee.ContactInformation == null)
                employee.ContactInformation = new ContactInformation();

            if (employee.GovernmentId == null)
                employee.GovernmentId = new GovernmentId();

            return new EmployeeProfileDto
            {
                EmployeeId = employee.EmployeeId,
                ErpUserId = employee.ErpUserId,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName ?? "",
                LastName = employee.LastName,
                Position = employee.EmploymentDetails?.Position ?? "",
                Department = employee.EmploymentDetails?.Department ?? "",
                Status = employee.Status,
                Role = employee.role?.Role_Description ?? "",
                Email = employee.ContactInformation?.EmailAddress ?? "",
                PhoneNumber = employee.ContactInformation?.PhoneNumber ?? "",
                DateOfBirth = employee.DateOfBirth.ToString("yyyy-MM-dd"),
                DateHired = employee.EmploymentDetails?.HireDate.ToString("yyyy-MM-dd") ?? "",
                AssignedLocation = employee.EmploymentDetails?.Department ?? "",
                Supervisor = "", // Not available in EmploymentDetails entity
                EmergencyContactName = employee.EmergencyContacts?.FirstOrDefault()?.FirstName + " " + employee.EmergencyContacts?.FirstOrDefault()?.LastName ?? "",
                EmergencyContactPhone = employee.EmergencyContacts?.FirstOrDefault()?.PhoneNumber ?? "",
                EmergencyContactAddress = employee.ContactInformation?.PresentAddress ?? "",
                EmergencyContactRelationship = employee.EmergencyContacts?.FirstOrDefault()?.Relationship ?? "",
                SSS = employee.GovernmentId?.SSS_Number ?? "",
                PhilHealth = employee.GovernmentId?.PhilHealth_Number ?? "",
                PagIbig = employee.GovernmentId?.HDMF_Number ?? "",
                TIN = employee.GovernmentId?.TIN_Number ?? "",
                NbiClearanceDate = employee.GovernmentId?.NbiClearanceDate?.ToString("yyyy-MM-dd") ?? "",
                BarangayClearanceDate = employee.GovernmentId?.BarangayClearanceDate?.ToString("yyyy-MM-dd") ?? "",
                BankDetails = "", // Not available in GovernmentId entity
                UniformIssued = false, // Not available in GovernmentId entity
                CompanyIdIssued = employee.CompanyProperty?.Id_Issue_Date != DateOnly.MinValue, // Not available in GovernmentId entity
                CompanyIdNumber = employee.CompanyProperty?.Employee_Id_Code ?? "", // Not available in GovernmentId entity
                EquipmentIssued = "", // Not available in GovernmentId entity
                CheckedBy = "", // Not available in GovernmentId entity
                CheckedDate = "", // Not available in GovernmentId entity
                Remarks = "" // Not available in GovernmentId entity
            };
        }

        public async Task<EmployeeProfileDto?> GetEmployeeByErpUserIdAsync(string erpUserId)
        {
            var employee = await _context.Employees
                .Include(e => e.EmploymentDetails)
                .Include(e => e.ContactInformation)
                .Include(e => e.GovernmentId)
                .Include(e => e.EmergencyContacts)
                .Include(e => e.CompanyProperty)
                .FirstOrDefaultAsync(e => e.ErpUserId == erpUserId);

            if (employee == null)
                return null;

            // Initialize missing entities to prevent null reference exceptions
            if (employee.EmploymentDetails == null)
                employee.EmploymentDetails = new EmploymentDetails();

            if (employee.ContactInformation == null)
                employee.ContactInformation = new ContactInformation();

            if (employee.GovernmentId == null)
                employee.GovernmentId = new GovernmentId();

            return new EmployeeProfileDto
            {
                EmployeeId = employee.EmployeeId,
                ErpUserId = employee.ErpUserId,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName ?? "",
                LastName = employee.LastName,
                Position = employee.EmploymentDetails?.Position ?? "",
                Department = employee.EmploymentDetails?.Department ?? "",
                Status = employee.Status,
                Email = employee.ContactInformation?.EmailAddress ?? "",
                PhoneNumber = employee.ContactInformation?.PhoneNumber ?? "",
                DateOfBirth = employee.DateOfBirth.ToString("yyyy-MM-dd"),
                DateHired = employee.EmploymentDetails?.HireDate.ToString("yyyy-MM-dd") ?? "",
                AssignedLocation = employee.EmploymentDetails?.Department ?? "",
                Supervisor = "", // Not available in EmploymentDetails entity
                EmergencyContactName = employee.EmergencyContacts?.FirstOrDefault()?.FirstName + " " + employee.EmergencyContacts?.FirstOrDefault()?.LastName ?? "",
                EmergencyContactPhone = employee.EmergencyContacts?.FirstOrDefault()?.PhoneNumber ?? "",
                EmergencyContactAddress = employee.ContactInformation?.PresentAddress ?? "",
                EmergencyContactRelationship = employee.EmergencyContacts?.FirstOrDefault()?.Relationship ?? "",
                SSS = employee.GovernmentId?.SSS_Number ?? "",
                PhilHealth = employee.GovernmentId?.PhilHealth_Number ?? "",
                PagIbig = employee.GovernmentId?.HDMF_Number ?? "",
                TIN = employee.GovernmentId?.TIN_Number ?? "",
                NbiClearanceDate = employee.GovernmentId?.NbiClearanceDate?.ToString("yyyy-MM-dd") ?? "",
                BarangayClearanceDate = employee.GovernmentId?.BarangayClearanceDate?.ToString("yyyy-MM-dd") ?? "",
                BankDetails = "", // Not available in GovernmentId entity
                UniformIssued = false, // Not available in GovernmentId entity
                CompanyIdIssued = employee.CompanyProperty?.Id_Issue_Date != DateOnly.MinValue, // Not available in GovernmentId entity
                CompanyIdNumber = employee.CompanyProperty?.Employee_Id_Code ?? "", // Not available in GovernmentId entity
                EquipmentIssued = "", // Not available in GovernmentId entity
                CheckedBy = "", // Not available in GovernmentId entity
                CheckedDate = "", // Not available in GovernmentId entity
                Remarks = "" // Not available in GovernmentId entity
            };
        }

        public async Task<bool> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Fetch existing employee with related entities
                var employee = await _context.Employees
                    .Include(e => e.EmploymentDetails)
                    .Include(e => e.ContactInformation)
                    .Include(e => e.GovernmentId)
                    .Include(e => e.EmergencyContacts)
                    .Include(e => e.CompanyProperty)
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                if (employee == null)
                    return false;

                // Update basic employee information
                if (!string.IsNullOrEmpty(dto.FirstName)) employee.FirstName = dto.FirstName;
                if (!string.IsNullOrEmpty(dto.MiddleName)) employee.MiddleName = dto.MiddleName;
                if (!string.IsNullOrEmpty(dto.LastName)) employee.LastName = dto.LastName;
                if (!string.IsNullOrEmpty(dto.Status)) employee.Status = dto.Status;
                if (!string.IsNullOrEmpty(dto.DateOfBirth) && DateOnly.TryParse(dto.DateOfBirth, out var dob))
                    employee.DateOfBirth = dob;

                // Update or create contact information
                if (employee.ContactInformation == null)
                {
                    employee.ContactInformation = new ContactInformation();
                    employee.ContactInformation.EmployeeId = employeeId;
                }

                if (!string.IsNullOrEmpty(dto.Email)) employee.ContactInformation.EmailAddress = dto.Email;
                if (!string.IsNullOrEmpty(dto.PhoneNumber)) employee.ContactInformation.PhoneNumber = dto.PhoneNumber;
                if (!string.IsNullOrEmpty(dto.EmergencyContactAddress)) employee.ContactInformation.PresentAddress = dto.EmergencyContactAddress;

                // Update or create emergency contact
                if (!string.IsNullOrEmpty(dto.EmergencyContactName))
                {
                    var existingEmergencyContact = employee.EmergencyContacts.FirstOrDefault();
                    if (existingEmergencyContact == null)
                    {
                        // Create new emergency contact
                        var emergencyContact = new EmergencyContact
                        {
                            EmployeeId = employeeId,
                            FirstName = dto.EmergencyContactName.Split(' ').FirstOrDefault() ?? dto.EmergencyContactName,
                            LastName = dto.EmergencyContactName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                            PhoneNumber = dto.EmergencyContactPhone,
                            Address = dto.EmergencyContactAddress ?? "Not specified",
                            Relationship = dto.EmergencyContactRelationship ?? "Not specified"
                        };
                        _context.EmergencyContacts.Add(emergencyContact);
                    }
                    else
                    {
                        // Update existing emergency contact - only update fields if values are provided
                        existingEmergencyContact.FirstName = dto.EmergencyContactName.Split(' ').FirstOrDefault() ?? dto.EmergencyContactName;
                        existingEmergencyContact.LastName = dto.EmergencyContactName.Split(' ').Skip(1).FirstOrDefault() ?? "";
                        if (!string.IsNullOrEmpty(dto.EmergencyContactPhone)) existingEmergencyContact.PhoneNumber = dto.EmergencyContactPhone;
                        if (!string.IsNullOrEmpty(dto.EmergencyContactAddress)) existingEmergencyContact.Address = dto.EmergencyContactAddress;
                        if (!string.IsNullOrEmpty(dto.EmergencyContactRelationship)) existingEmergencyContact.Relationship = dto.EmergencyContactRelationship;
                    }
                }

                // Update or create employment details
                if (employee.EmploymentDetails == null)
                {
                    employee.EmploymentDetails = new EmploymentDetails();
                    employee.EmploymentDetails.EmployeeId = employeeId;
                }

                if (!string.IsNullOrEmpty(dto.Position)) employee.EmploymentDetails.Position = dto.Position;
                if (!string.IsNullOrEmpty(dto.Department)) employee.EmploymentDetails.Department = dto.Department;
                if (!string.IsNullOrEmpty(dto.AssignedLocation)) employee.EmploymentDetails.Department = dto.AssignedLocation;
                if (!string.IsNullOrEmpty(dto.DateHired) && DateOnly.TryParse(dto.DateHired, out var dateHired))
                    employee.EmploymentDetails.HireDate = dateHired;
                // Note: Supervisor is not available in EmploymentDetails entity

                // Update or create government IDs
                if (employee.GovernmentId == null)
                {
                    employee.GovernmentId = new GovernmentId();
                    employee.GovernmentId.Employee_Id = employeeId;
                    // Initialize with default values to prevent null constraint violations
                    employee.GovernmentId.SSS_Number = dto.SSS ?? "";
                    employee.GovernmentId.PhilHealth_Number = dto.PhilHealth ?? "";
                    employee.GovernmentId.HDMF_Number = dto.PagIbig ?? "";
                    employee.GovernmentId.TIN_Number = dto.TIN ?? "";
                    if (!string.IsNullOrEmpty(dto.NbiClearanceDate) && DateOnly.TryParse(dto.NbiClearanceDate, out var nbiDate))
                        employee.GovernmentId.NbiClearanceDate = nbiDate;
                    if (!string.IsNullOrEmpty(dto.BarangayClearanceDate) && DateOnly.TryParse(dto.BarangayClearanceDate, out var barangayDate))
                        employee.GovernmentId.BarangayClearanceDate = barangayDate;
                }
                else
                {
                    // Update existing GovernmentId only if values are provided
                    if (!string.IsNullOrEmpty(dto.SSS)) employee.GovernmentId.SSS_Number = dto.SSS;
                    if (!string.IsNullOrEmpty(dto.PhilHealth)) employee.GovernmentId.PhilHealth_Number = dto.PhilHealth;
                    if (!string.IsNullOrEmpty(dto.PagIbig)) employee.GovernmentId.HDMF_Number = dto.PagIbig;
                    if (!string.IsNullOrEmpty(dto.TIN)) employee.GovernmentId.TIN_Number = dto.TIN;
                    if (!string.IsNullOrEmpty(dto.NbiClearanceDate) && DateOnly.TryParse(dto.NbiClearanceDate, out var nbiDate))
                        employee.GovernmentId.NbiClearanceDate = nbiDate;
                    if (!string.IsNullOrEmpty(dto.BarangayClearanceDate) && DateOnly.TryParse(dto.BarangayClearanceDate, out var barangayDate))
                        employee.GovernmentId.BarangayClearanceDate = barangayDate;
                }

                // Update or create company property
                if (!string.IsNullOrEmpty(dto.CompanyIdNumber))
                {
                    if (employee.CompanyProperty == null)
                    {
                        employee.CompanyProperty = new CompanyProperty
                        {
                            Employee_Id = employeeId,
                            Employee_Id_Code = dto.CompanyIdNumber,
                            Id_Issue_Date = dto.CompanyIdIssued == true ? DateOnly.FromDateTime(DateTime.Now) : DateOnly.MinValue
                        };
                        _context.CompanyProperties.Add(employee.CompanyProperty);
                    }
                    else
                    {
                        employee.CompanyProperty.Employee_Id_Code = dto.CompanyIdNumber;
                        if (dto.CompanyIdIssued == true && employee.CompanyProperty.Id_Issue_Date == DateOnly.MinValue)
                        {
                            employee.CompanyProperty.Id_Issue_Date = DateOnly.FromDateTime(DateTime.Now);
                        }
                    }
                }

                // Process new document files
                await ProcessDocumentFiles(employeeId, dto);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully updated employee with ID: {EmployeeId}", employeeId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating employee with ID: {EmployeeId}", employeeId);
                return false;
            }
        }

        private async Task ProcessDocumentFiles(int employeeId, UpdateEmployeeDto dto)
        {
            var documentMappings = new Dictionary<string, string>
            {
                { nameof(dto.ResumeFile), "Resume" },
                { nameof(dto.PersonalDataSheetFile), "Personal Data Sheet" },
                { nameof(dto.IdPictureFile), "ID Picture" },
                { nameof(dto.BirthCertificateFile), "Birth Certificate" },
                { nameof(dto.MarriageCertificateFile), "Marriage Certificate" },
                { nameof(dto.JobDescriptionFile), "Job Description" },
                { nameof(dto.EmploymentContractFile), "Employment Contract" },
                { nameof(dto.CompanyRulesFile), "Company Rules" },
                { nameof(dto.NdaFile), "NDA" },
                { nameof(dto.HandbookFile), "Handbook" },
                { nameof(dto.SalaryAgreementFile), "Salary Agreement" },
                { nameof(dto.Bir2316File), "BIR 2316" },
                { nameof(dto.AttendanceRecordFile), "Attendance Record" },
                { nameof(dto.AcknowledgmentReceiptFile), "Acknowledgment Receipt" },
                { nameof(dto.MedicalCertificateFile), "Medical Certificate" },
                { nameof(dto.DrugTestFile), "Drug Test" },
                { nameof(dto.VaccinationCardFile), "Vaccination Card" },
                { nameof(dto.PerformanceEvaluationFile), "Performance Evaluation" },
                { nameof(dto.IncidentReportFile), "Incident Report" },
                { nameof(dto.DisciplinaryRecordFile), "Disciplinary Record" },
                { nameof(dto.PromotionRecordFile), "Promotion Record" }
            };

            foreach (var mapping in documentMappings)
            {
                var fileProperty = typeof(UpdateEmployeeDto).GetProperty(mapping.Key);
                var file = fileProperty?.GetValue(dto) as IFormFile;

                if (file != null && file.Length > 0)
                {
                    // Here you would typically save the file to storage and get the URL
                    // For now, we'll create a placeholder URL
                    var fileUrl = $"/uploads/employee_{employeeId}/{mapping.Value}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";

                    var document = new EmployeeDocument
                    {
                        EmployeeId = employeeId,
                        DocumentName = mapping.Value,
                        DocumentType = mapping.Value,
                        FileUrl = fileUrl,
                        UploadDate = DateTime.UtcNow,
                        Issue_Date = DateOnly.FromDateTime(DateTime.UtcNow),
                        Expiry_Date = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)) // Default 1 year expiry
                    };

                    _context.EmployeeDocuments.Add(document);
                }
            }
        }

        public async Task<List<UnregisteredEmployeeDto>> GetUnregisteredEmployeesAsync()
        {
            try
            {
                // Log total employees count for debugging
                var totalEmployees = await _context.Employees.CountAsync();
                _logger.LogInformation("Total employees in database: {count}", totalEmployees);

                var employees = await _context.Employees
                    .Include(e => e.EmploymentDetails)
                    .Include(e => e.ContactInformation)
                    .Where(e => string.IsNullOrEmpty(e.ErpUserId) || 
                               e.ErpUserId == "00000000-0000-0000-0000-000000000000" || 
                               e.ErpUserId == "null" ||
                               e.ErpUserId == "NULL" ||
                               e.ErpUserId == "")
                    .Select(e => new UnregisteredEmployeeDto
                    {
                        employeeId = e.EmployeeId,
                        firstName = e.FirstName ?? string.Empty,
                        lastName = e.LastName ?? string.Empty,
                        emailAddress = e.ContactInformation != null ? (e.ContactInformation.EmailAddress ?? string.Empty) : string.Empty,
                        department = e.EmploymentDetails != null ? (e.EmploymentDetails.Department ?? string.Empty) : string.Empty,
                        position = e.EmploymentDetails != null ? (e.EmploymentDetails.Position ?? string.Empty) : string.Empty
                    })
                    .OrderBy(e => e.lastName)
                    .ThenBy(e => e.firstName)
                    .ToListAsync();

                // Log for debugging
                _logger.LogInformation("Found {count} unregistered employees out of {total}", employees.Count, totalEmployees);
                foreach (var emp in employees.Take(5)) // Log first 5 for debugging
                {
                    _logger.LogInformation("Employee: {employeeId} - {firstName} {lastName}, Email: {emailAddress}, Dept: {department}, Pos: {position}", 
                        emp.employeeId, emp.firstName, emp.lastName, emp.emailAddress, emp.department, emp.position);
                }

                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unregistered employees");
                return new List<UnregisteredEmployeeDto>();
            }
        }

        public async Task<bool> UpdateEmployeeErpUserIdAsync(int employeeId, string erpUserId)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null) return false;

                employee.ErpUserId = erpUserId;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully updated ErpUserId for employee {EmployeeId} to {ErpUserId}", employeeId, erpUserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ErpUserId for employee {EmployeeId}", employeeId);
                return false;
            }
        }
    }
}

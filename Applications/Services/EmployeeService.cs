using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Applications.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Applications.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(hrmAppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.role)
                .Include(e => e.EmergencyContacts)
                .Include(e => e.GovernmentId)
                .Include(e => e.CompanyProperty)
                .Include(e => e.DocumentStatuses)
                .Include(e => e.EmploymentDetails)
                .Include(e => e.ContactInformation)
                .ToListAsync();

            return employees.Select(e => MapToEmployeeDto(e));
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.role)
                .Include(e => e.EmergencyContacts)
                .Include(e => e.GovernmentId)
                .Include(e => e.CompanyProperty)
                .Include(e => e.DocumentStatuses)
                .Include(e => e.Documents)
                .Include(e => e.Exits)
                .Include(e => e.EmploymentDetails)
                .Include(e => e.ContactInformation)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return null;

            return MapToEmployeeDto(employee);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeCreateDto createDto)
        {
            var employee = new Employee
            {
                FirstName = createDto.firstName,
                LastName = createDto.lastName,
                DateOfBirth = ParseDateOnly(createDto.dateOfBirth),
                Gender = createDto.gender,
                CivilStatus = createDto.civilStatus,
                BloodType = createDto.bloodType,
                Status = createDto.status,
                Role_ID = createDto.roleId,
                AvatarIndex = 0,
                ErpUserId = Guid.NewGuid().ToString() // Will be updated by authentication service
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Create related entities
            var employmentDetails = new EmploymentDetails
            {
                EmployeeId = employee.EmployeeId,
                HireDate = ParseDateOnly(createDto.hireDate),
                Position = createDto.position,
                Department = createDto.department,
                BasePay = 0, // Will be updated in payroll phase
                EmploymentStatus = "Regular" // Default status
            };
            _context.EmploymentDetails.Add(employmentDetails);

            var contactInformation = new ContactInformation
            {
                EmployeeId = employee.EmployeeId,
                EmailAddress = createDto.email,
                PhoneNumber = createDto.phone,
                PresentAddress = createDto.address
            };
            _context.ContactInformation.Add(contactInformation);

            if (createDto.emergencyContact != null)
            {
                var emergencyContact = new EmergencyContact
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = createDto.emergencyContact.name.Split(' ').FirstOrDefault() ?? "",
                    LastName = createDto.emergencyContact.name.Split(' ').Skip(1).FirstOrDefault() ?? "",
                    Relationship = createDto.emergencyContact.relationship,
                    PhoneNumber = createDto.emergencyContact.phone
                };
                _context.EmergencyContacts.Add(emergencyContact);
            }

            if (createDto.governmentIds != null)
            {
                var governmentId = new GovernmentId
                {
                    Employee_Id = employee.EmployeeId,
                    SSS_Number = createDto.governmentIds.sss,
                    PhilHealth_Number = createDto.governmentIds.philHealth,
                    TIN_Number = createDto.governmentIds.tin,
                    HDMF_Number = createDto.governmentIds.hdmf
                };
                _context.GovernmentIds.Add(governmentId);
            }

            if (createDto.companyProperty != null)
            {
                var companyProperty = new CompanyProperty
                {
                    Employee_Id = employee.EmployeeId,
                    Employee_Id_Code = createDto.companyProperty.employeeId,
                    Id_Issue_Date = ParseDateOnly(createDto.companyProperty.idIssueDate),
                    Uniform_Top_Size = createDto.companyProperty.uniformSize.top,
                    Uniform_Bottom_Size = createDto.companyProperty.uniformSize.bottom,
                    Uniform_Shoe_Size = createDto.companyProperty.uniformSize.shoes,
                    Uniform_Issue_Date = ParseDateOnly(createDto.companyProperty.uniformIssueDate),
                    Equipment_JSON = JsonSerializer.Serialize(createDto.companyProperty.equipment),
                    Return_Date = string.IsNullOrEmpty(createDto.companyProperty.returnDate) ? null : ParseDateOnly(createDto.companyProperty.returnDate)
                };
                _context.CompanyProperties.Add(companyProperty);
            }

            // Initialize document statuses
            var documentTypes = new[] { "personal", "government", "company", "performance" };
            foreach (var docType in documentTypes)
            {
                _context.DocumentStatuses.Add(new DocumentStatus
                {
                    Employee_Id = employee.EmployeeId,
                    Document_Type = docType,
                    Is_Completed = false,
                    Last_Updated = DateOnly.FromDateTime(DateTime.UtcNow)
                });
            }

            await _context.SaveChangesAsync();
            await _context.Entry(employee).Reference(e => e.role).LoadAsync();

            return MapToEmployeeDto(employee);
        }

        public async Task<bool> UpdateEmployeeProfileAsync(int id, EmployeeUpdateDto updateDto)
        {
            var employee = await _context.Employees
                .Include(e => e.EmergencyContacts)
                .Include(e => e.GovernmentId)
                .Include(e => e.CompanyProperty)
                .Include(e => e.DocumentStatuses)
                .Include(e => e.EmploymentDetails)
                .Include(e => e.ContactInformation)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return false;

            // Capture old state for audit trail (limited to 255 chars)
            var oldStateJson = JsonSerializer.Serialize(employee);
            var truncatedOldState = oldStateJson.Length > 255 ? oldStateJson.Substring(0, 255) : oldStateJson;

            // Update core employee fields
            if (updateDto.firstName != null) employee.FirstName = updateDto.firstName;
            if (updateDto.lastName != null) employee.LastName = updateDto.lastName;
            // TODO: Fix these property assignments after migration
            // These properties have been moved to EmploymentDetails and ContactInformation entities
            // if (updateDto.position != null) employee.EmploymentDetails.Position = updateDto.position;
            // if (updateDto.department != null) employee.EmploymentDetails.Department = updateDto.department;
            // if (updateDto.email != null) employee.ContactInformation.EmailAddress = updateDto.email;
            // if (updateDto.phone != null) employee.ContactInformation.PhoneNumber = updateDto.phone;
            // if (updateDto.address != null) employee.ContactInformation.PresentAddress = updateDto.address;
            if (updateDto.dateOfBirth != null) employee.DateOfBirth = ParseDateOnly(updateDto.dateOfBirth);
            if (updateDto.gender != null) employee.Gender = updateDto.gender;
            if (updateDto.civilStatus != null) employee.CivilStatus = updateDto.civilStatus;
            if (updateDto.bloodType != null) employee.BloodType = updateDto.bloodType;
            // TODO: Fix these legacy properties after migration
            // if (updateDto.paymentMethod != null) employee.PaymentMethod = updateDto.paymentMethod;
            // if (updateDto.accountNumber != null) employee.AccountNumber = updateDto.accountNumber;
            if (updateDto.status != null) employee.Status = updateDto.status;
            if (updateDto.roleId.HasValue) employee.Role_ID = updateDto.roleId.Value;
            // if (updateDto.hireDate != null) employee.Hire_Date = ParseDateOnly(updateDto.hireDate);

            // TODO: Fix EmergencyContact update after migration - now 1-to-Many relationship
            // if (updateDto.emergencyContact != null)
            // {
            //     // Handle 1-to-Many emergency contacts update
            // }

            // Upsert GovernmentId
            if (updateDto.governmentIds != null)
            {
                if (employee.GovernmentId == null)
                {
                    employee.GovernmentId = new GovernmentId
                    {
                        Employee_Id = employee.EmployeeId,
                        SSS_Number = updateDto.governmentIds.sss,
                        PhilHealth_Number = updateDto.governmentIds.philHealth,
                        TIN_Number = updateDto.governmentIds.tin,
                        HDMF_Number = updateDto.governmentIds.hdmf
                    };
                    _context.GovernmentIds.Add(employee.GovernmentId);
                }
                else
                {
                    employee.GovernmentId.SSS_Number = updateDto.governmentIds.sss;
                    employee.GovernmentId.PhilHealth_Number = updateDto.governmentIds.philHealth;
                    employee.GovernmentId.TIN_Number = updateDto.governmentIds.tin;
                    employee.GovernmentId.HDMF_Number = updateDto.governmentIds.hdmf;
                }
            }

            // Upsert CompanyProperty
            if (updateDto.companyProperty != null)
            {
                if (employee.CompanyProperty == null)
                {
                    employee.CompanyProperty = new CompanyProperty
                    {
                        Employee_Id = employee.EmployeeId,
                        Employee_Id_Code = updateDto.companyProperty.employeeId,
                        Id_Issue_Date = ParseDateOnly(updateDto.companyProperty.idIssueDate),
                        Uniform_Top_Size = updateDto.companyProperty.uniformSize.top,
                        Uniform_Bottom_Size = updateDto.companyProperty.uniformSize.bottom,
                        Uniform_Shoe_Size = updateDto.companyProperty.uniformSize.shoes,
                        Uniform_Issue_Date = ParseDateOnly(updateDto.companyProperty.uniformIssueDate),
                        Equipment_JSON = JsonSerializer.Serialize(updateDto.companyProperty.equipment),
                        Return_Date = string.IsNullOrEmpty(updateDto.companyProperty.returnDate) ? null : ParseDateOnly(updateDto.companyProperty.returnDate)
                    };
                    _context.CompanyProperties.Add(employee.CompanyProperty);
                }
                else
                {
                    employee.CompanyProperty.Employee_Id_Code = updateDto.companyProperty.employeeId;
                    employee.CompanyProperty.Id_Issue_Date = ParseDateOnly(updateDto.companyProperty.idIssueDate);
                    employee.CompanyProperty.Uniform_Top_Size = updateDto.companyProperty.uniformSize.top;
                    employee.CompanyProperty.Uniform_Bottom_Size = updateDto.companyProperty.uniformSize.bottom;
                    employee.CompanyProperty.Uniform_Shoe_Size = updateDto.companyProperty.uniformSize.shoes;
                    employee.CompanyProperty.Uniform_Issue_Date = ParseDateOnly(updateDto.companyProperty.uniformIssueDate);
                    employee.CompanyProperty.Equipment_JSON = JsonSerializer.Serialize(updateDto.companyProperty.equipment);
                    employee.CompanyProperty.Return_Date = string.IsNullOrEmpty(updateDto.companyProperty.returnDate) ? null : ParseDateOnly(updateDto.companyProperty.returnDate);
                }
            }

            // Capture new state for audit trail (limited to 255 chars)
            var newStateJson = JsonSerializer.Serialize(employee);
            var truncatedNewState = newStateJson.Length > 255 ? newStateJson.Substring(0, 255) : newStateJson;

            // Get current user from JWT claims
            var user = _httpContextAccessor.HttpContext?.User;
            var changedBy = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? user?.FindFirst("sub")?.Value 
                ?? user?.FindFirst(ClaimTypes.Email)?.Value 
                ?? "System";

            // Create audit history record
            var history = new EmployeeHistory
            {
                Employee_ID = employee.EmployeeId,
                Action_Type = "Update",
                Old_Value = truncatedOldState,
                New_Value = truncatedNewState,
                Changed_By = changedBy,
                Changed_At = DateTime.UtcNow
            };

            _context.EmployeeHistories.Add(history);
            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            return await _context.SaveChangesAsync() > 0;
        }

        private EmployeeDto MapToEmployeeDto(Employee employee)
        {
            var fullName = $"{employee.FirstName} {employee.LastName}";
            var initials = GetInitials(employee.FirstName, employee.LastName);

            return new EmployeeDto
            {
                id = employee.EmployeeId,
                name = fullName,
                initials = initials,
                position = employee.EmploymentDetails?.Position ?? "",
                department = employee.EmploymentDetails?.Department ?? "",
                hireDate = employee.EmploymentDetails?.HireDate.ToString("yyyy-MM-dd") ?? "",
                status = employee.Status,
                email = employee.ContactInformation?.EmailAddress ?? "",
                phone = employee.ContactInformation?.PhoneNumber ?? "",
                avatarIndex = employee.AvatarIndex,
                documents = MapDocumentStatuses(employee.DocumentStatuses),
                address = employee.ContactInformation?.PresentAddress ?? "",
                emergencyContact = employee.EmergencyContacts?.FirstOrDefault() != null ? new EmergencyContactDto
                {
                    name = $"{employee.EmergencyContacts.First().FirstName} {employee.EmergencyContacts.First().LastName}",
                    relationship = employee.EmergencyContacts.First().Relationship,
                    phone = employee.EmergencyContacts.First().PhoneNumber
                } : null,
                dateOfBirth = employee.DateOfBirth.ToString("yyyy-MM-dd"),
                gender = employee.Gender,
                civilStatus = employee.CivilStatus,
                bloodType = employee.BloodType,
                governmentIds = employee.GovernmentId != null ? new GovernmentIdsDto
                {
                    sss = employee.GovernmentId.SSS_Number,
                    philHealth = employee.GovernmentId.PhilHealth_Number,
                    tin = employee.GovernmentId.TIN_Number,
                    hdmf = employee.GovernmentId.HDMF_Number
                } : null,
                companyProperty = employee.CompanyProperty != null ? new CompanyPropertyDto
                {
                    employeeId = employee.CompanyProperty.Employee_Id_Code,
                    idIssueDate = employee.CompanyProperty.Id_Issue_Date.ToString("yyyy-MM-dd"),
                    uniformSize = new UniformSizeDto
                    {
                        top = employee.CompanyProperty.Uniform_Top_Size,
                        bottom = employee.CompanyProperty.Uniform_Bottom_Size,
                        shoes = employee.CompanyProperty.Uniform_Shoe_Size
                    },
                    uniformIssueDate = employee.CompanyProperty.Uniform_Issue_Date.ToString("yyyy-MM-dd"),
                    equipment = JsonSerializer.Deserialize<string[]>(employee.CompanyProperty.Equipment_JSON) ?? new string[0],
                    returnDate = employee.CompanyProperty.Return_Date?.ToString("yyyy-MM-dd")
                } : null,
                attendanceSummary = new AttendanceSummaryDto
                {
                    attendanceRate = 0, // TODO: Calculate from attendance logs
                    tardinessCount = 0, // TODO: Calculate from attendance logs
                    leaveBalance = 0, // TODO: Calculate from leave requests
                    totalDaysWorked = 0 // TODO: Calculate from attendance logs
                }
            };
        }

        private DocumentStatusDto MapDocumentStatuses(ICollection<DocumentStatus> documentStatuses)
        {
            var result = new DocumentStatusDto();
            var docDict = documentStatuses.ToDictionary(ds => ds.Document_Type.ToLower());

            foreach (var docType in new[] { "personal", "government", "company", "performance" })
            {
                if (docDict.TryGetValue(docType, out var status))
                {
                    var item = new DocumentStatusItemDto
                    {
                        completed = status.Is_Completed,
                        lastUpdated = status.Last_Updated.ToString("yyyy-MM-dd"),
                        expiryDate = status.Expiry_Date?.ToString("yyyy-MM-dd")
                    };

                    switch (docType)
                    {
                        case "personal": result.personal = item; break;
                        case "government": result.government = item; break;
                        case "company": result.company = item; break;
                        case "performance": result.performance = item; break;
                    }
                }
                else
                {
                    var item = new DocumentStatusItemDto
                    {
                        completed = false,
                        lastUpdated = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
                        expiryDate = null
                    };

                    switch (docType)
                    {
                        case "personal": result.personal = item; break;
                        case "government": result.government = item; break;
                        case "company": result.company = item; break;
                        case "performance": result.performance = item; break;
                    }
                }
            }

            return result;
        }

        private string GetInitials(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName)) return "??";
            var first = string.IsNullOrEmpty(firstName) ? "" : firstName.Substring(0, 1).ToUpper();
            var last = string.IsNullOrEmpty(lastName) ? "" : lastName.Substring(0, 1).ToUpper();
            return $"{first}{last}";
        }

        private DateOnly ParseDateOnly(string dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return DateOnly.FromDateTime(DateTime.UtcNow);
            if (DateOnly.TryParse(dateString, out var date)) return date;
            return DateOnly.FromDateTime(DateTime.UtcNow);
        }
    }
}
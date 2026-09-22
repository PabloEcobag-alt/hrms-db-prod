using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Applications.Commands;
using Applications.Events;
using Applications.Interfaces;
using Api.Contracts.Auth;

namespace Applications.Handlers
{
    public class HireApplicantCommandHandler : IRequestHandler<HireApplicantCommand, EmployeeReadDto?>
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<HireApplicantCommandHandler> _logger;
        private readonly IAuthServiceClient _authServiceClient;

        public HireApplicantCommandHandler(
            hrmAppDbContext context,
            IMapper mapper,
            IMediator mediator,
            ILogger<HireApplicantCommandHandler> logger,
            IAuthServiceClient authServiceClient)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
            _authServiceClient = authServiceClient;
        }

        public async Task<EmployeeReadDto?> Handle(HireApplicantCommand request, CancellationToken cancellationToken)
        {
            var applicant = await _context.Applicants.FindAsync(request.ApplicantId);
            if (applicant == null) return null;

            // 1. Preemptive Validation
            if (applicant.Hiring_Stage == "Hired" || applicant.Hiring_Stage == "Probationary")
                throw new ArgumentException("This applicant has already been transformed into an employee.");

            if (!string.IsNullOrWhiteSpace(applicant.Email))
            {
                bool emailExists = await _context.ContactInformation.AnyAsync(c => c.EmailAddress == applicant.Email, cancellationToken);
                if (emailExists) throw new ArgumentException($"An employee with the email '{applicant.Email}' already exists in the HRMS database.");
            }

            // 2. Transaction Wrapper
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
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
                    ErpUserId = null // Will be set during registration below
                };

                // Provision User synchronously via Token Relay
                var createUserRequest = new CreateUserRequest
                {
                    FirstName = applicant.First_Name,
                    LastName = applicant.Last_Name,
                    Email = applicant.Email ?? string.Empty,
                    Role = "Staff/Employee",
                    Apps = new List<AppPermissionDto>
                    {
                        new AppPermissionDto
                        {
                            AppName = "HRMS",
                            Modules = new List<ModulePermissionDto>
                            {
                                new ModulePermissionDto { ModuleName = "Employee Information", CanRead = true }
                            }
                        }
                    }
                };
                
                string authUserId;
                try
                {
                    authUserId = await _authServiceClient.ProvisionUserAsync(createUserRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Auth provisioning failed for applicant {ApplicantId}", request.ApplicantId);
                    throw new InvalidOperationException($"Auth Provisioning Failed: {ex.Message}", ex);
                }
                newEmployee.ErpUserId = authUserId;

                _context.Employees.Add(newEmployee);
                await _context.SaveChangesAsync(cancellationToken);

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
                    HireDate = DateOnly.FromDateTime(request.StartDate),
                    EmploymentStatus = request.HiringStage, // Use the hiring stage from payload
                    ProbationaryEndDate = request.ProbationaryEndDate.HasValue 
                        ? DateOnly.FromDateTime(request.ProbationaryEndDate.Value) 
                        : null,
                    BasePay = 0
                };
                _context.EmploymentDetails.Add(employmentDetails);

                // Update applicant's hiring stage to match payload
                applicant.Hiring_Stage = request.HiringStage;
                applicant.Expected_Start_Date = DateOnly.FromDateTime(request.StartDate);
                if (request.ProbationaryEndDate.HasValue)
                {
                    applicant.Probationary_End_Date = DateOnly.FromDateTime(request.ProbationaryEndDate.Value);
                }
                
                await _context.SaveChangesAsync(cancellationToken);

                await _context.Entry(newEmployee).Reference(e => e.role).LoadAsync(cancellationToken);

                var result = await _context.Employees
                    .Include(e => e.role)
                    .FirstOrDefaultAsync(e => e.EmployeeId == newEmployee.EmployeeId, cancellationToken);

                var employeeDto = _mapper.Map<EmployeeReadDto>(result);

                // Commit transaction BEFORE publishing events
                await transaction.CommitAsync(cancellationToken);

                // Publish the event after successful transaction
                await _mediator.Publish(new ApplicantHiredEvent
                {
                    ApplicantId = request.ApplicantId,
                    EmployeeId = newEmployee.EmployeeId,
                    HiredAt = DateTime.UtcNow,
                    Email = applicant.Email ?? string.Empty,
                    Phone = applicant.Mobile ?? applicant.Contact_Details ?? string.Empty,
                    Position = applicant.Position ?? string.Empty
                }, cancellationToken);

                _logger.LogInformation("Successfully hired applicant {ApplicantId} as employee {EmployeeId}", 
                    request.ApplicantId, newEmployee.EmployeeId);

                return employeeDto;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; // Bubble up the error to the controller
            }
        }
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Applications.Commands;
using Applications.Events;

namespace Applications.Handlers
{
    public class HireApplicantCommandHandler : IRequestHandler<HireApplicantCommand, EmployeeReadDto?>
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<HireApplicantCommandHandler> _logger;

        public HireApplicantCommandHandler(
            hrmAppDbContext context,
            IMapper mapper,
            IMediator mediator,
            ILogger<HireApplicantCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<EmployeeReadDto?> Handle(HireApplicantCommand request, CancellationToken cancellationToken)
        {
            var applicant = await _context.Applicants.FindAsync(request.ApplicantId);
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
    }
}
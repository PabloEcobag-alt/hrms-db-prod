using AutoMapper;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Api.Contracts.RoleExit;
using Api.Contracts.Document;
using Api.Contracts.Checklist;
using Api.Contracts.Applicant;
using Api.Contracts.Attendance;
using Api.Contracts.Leave;
using Api.Contracts.CashAdvance;
using Api.Contracts.LeaveType;
using Api.Contracts.Payroll;

namespace ApiHrm.Infrastructures.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDetailDto>();
            CreateMap<ApplicantCreateDto, Applicant>();
            CreateMap<Applicant, ApplicantReadDto>()
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.First_Name} {src.Last_Name}"))
                .ForMember(dest => dest.Position,
                opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Phone,
                opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Hiring_Stage,
                opt => opt.MapFrom(src => src.Hiring_Stage))
                .ForMember(dest => dest.Source,
                opt => opt.MapFrom(src => src.Source))
                .ForMember(dest => dest.Interview_Date,
                opt => opt.MapFrom(src => src.Interview_Date))
                .ForMember(dest => dest.Expected_Start_Date,
                opt => opt.MapFrom(src => src.Expected_Start_Date));

            CreateMap<DocumentCreateDto, Document>();
            CreateMap<Employee, EmployeeReadDto>()
                .MaxDepth(3)
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.role.Role_Description));

            CreateMap<EmployeeCreateDto, Employee>()
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore());

            CreateMap<EmployeeUpdateDto, Employee>();

            CreateMap<RoleCreateDto, Role>();
            CreateMap<Role, RoleReadDto>();

            CreateMap<ExitCreateDto, Exit>();
            CreateMap<Exit, ExitReadDto>()
                .ForMember(dest => dest.EmployeeFullName, 
                opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.LastName}"));

            CreateMap<ChecklistUpdateDto, Checklist>();
            CreateMap<Checklist, ChecklistReadDto>();

            CreateMap<DocumentUploadDto, Document>();
            CreateMap<Document, DocumentReadDto>();

            // Reference Data
            CreateMap<LeaveType, LeaveTypeReadDto>();

            // Sprint 2 — Attendance
            CreateMap<AttendanceLog, AttendanceLogReadDto>();

            // Sprint 2 — Leave
            CreateMap<LeaveRequest, LeaveRequestReadDto>()
                .ForMember(dest => dest.LeaveTypeLabel,
                    opt => opt.MapFrom(src => src.LeaveType != null ? src.LeaveType.LeaveTypeName : string.Empty));

            // Sprint 2 — Cash Advance
            CreateMap<CashAdvance, CashAdvanceReadDto>();

            // Sprint 3 — Payroll
            CreateMap<EmployeePayrollRecord, PayrollRecordReadDto>();

        }
    }
}
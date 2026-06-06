using AutoMapper;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Api.Contracts.RoleExit;
using Api.Contracts.Document;
using Api.Contracts.Checklist;
using Api.Contracts.Applicant;

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
                opt => opt.MapFrom(src => $"{src.First_Name} {src.Last_Name}"));

            CreateMap<DocumentCreateDto, Document>();
            CreateMap<Employee, EmployeeReadDto>()
                .MaxDepth(3)
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.First_Name} {src.Last_Name}"))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.role.Role_Description));

            CreateMap<EmployeeCreateDto, Employee>()
                .ForMember(dest => dest.Employee_Id, opt => opt.Ignore());

            CreateMap<EmployeeUpdateDto, Employee>();

            CreateMap<RoleCreateDto, Role>();
            CreateMap<Role, RoleReadDto>();

            CreateMap<ExitCreateDto, Exit>();
            CreateMap<Exit, ExitReadDto>()
                .ForMember(dest => dest.EmployeeFullName, 
                opt => opt.MapFrom(src => $"{src.Employee.First_Name} {src.Employee.Last_Name}"));

            CreateMap<ChecklistUpdateDto, Checklist>();
            CreateMap<Checklist, ChecklistReadDto>();

            CreateMap<DocumentUploadDto, Document>();
            CreateMap<Document, DocumentReadDto>();

        }
    }
}
using MediatR;
using Api.Contracts.Employee;
using Api.Contracts.Applicant;

namespace Applications.Commands
{
    public record HireApplicantCommand : IRequest<EmployeeReadDto>
    {
        public int ApplicantId { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime? ProbationaryEndDate { get; init; }
        public string HiringStage { get; init; } = "Regular";
    }
}
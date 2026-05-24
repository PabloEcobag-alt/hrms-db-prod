using Api.Contracts.Checklist;
using Api.Contracts.Document;

namespace Api.Contracts.Applicant
{
    public class ApplicantDetailDto
    {
        public int Applicant_Id { get; set; }
        public string First_Name { get; set; } = default!;
        public string Last_Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Resume_URL { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime Application_Date { get; set; }
        public List<DocumentReadDto> Documents { get; set; } = new();
        public List<ChecklistReadDto> Checklists { get; set; } = new();
    }
}
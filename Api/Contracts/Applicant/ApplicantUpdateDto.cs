namespace Api.Contracts.Applicant
{
    public class ApplicantUpdateDto
    {
        public string? Hiring_Stage { get; set; }
        public DateOnly? Interview_Date { get; set; }
        public DateOnly? Expected_Start_Date { get; set; }
        public string? Status { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}

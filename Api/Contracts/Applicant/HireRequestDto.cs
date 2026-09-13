namespace Api.Contracts.Applicant
{
    public class HireApplicantDto
    {
        public int ApplicantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ProbationaryEndDate { get; set; }
        public string HiringStage { get; set; } = string.Empty;
    }
}
namespace Api.Contracts.Analytics
{
    public class TopCandidateDto
    {
        public int ApplicantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public double MatchScore { get; set; }
        public string ScreeningResult { get; set; } = string.Empty;
        public string ModelVersion { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

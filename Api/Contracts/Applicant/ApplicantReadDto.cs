namespace Api.Contracts.Applicant
{
    public class ApplicantReadDto
    {
        public int Applicant_Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Status { get; set; } = default!; // e.g., "Pending", "Interviewed", "Hired"
        public string Resume_URL { get; set; } = default!;
        public string? Contact_Details { get; set; }
        public string? Payment_Method { get; set; }
        public DateTime Application_Date { get; set; }
    }
}
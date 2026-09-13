namespace Api.Contracts.Applicant
{
    public class ApplicantReadDto
    {
        public int Applicant_Id { get; set; }
        public string First_Name { get; set; } = default!;
        public string? Middle_Name { get; set; }
        public string Last_Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Status { get; set; } = default!; // e.g., "Pending", "Interviewed", "Hired"
        public string? Resume_URL { get; set; }
        public string? Contact_Details { get; set; }
        public string? Payment_Method { get; set; }
        public DateOnly Application_Date { get; set; }
        public string? Position { get; set; }
        public string? Mobile { get; set; }
        public string? Hiring_Stage { get; set; }
        public string? Source { get; set; }
        public DateOnly? Interview_Date { get; set; }
        public DateOnly? Expected_Start_Date { get; set; }
        public DateOnly? Probationary_End_Date { get; set; }
        public string? Interview_Notes { get; set; }
        
        // AI Scoring Fields (populated asynchronously)
        public double? Ai_Match_Score { get; set; }
        public string? Screening_Result { get; set; } = "Pending"; // Default: "Pending" | "Qualified" | "Rejected"
    }
}
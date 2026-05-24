namespace Api.Contracts.Document
{
    public class DocumentReadDto
    {
        public int Document_Id { get; set; }
        public string File_URL { get; set; } = default!;        
        public string Doc_Type { get; set; } = default!;        
        public DateTime Upload_Date { get; set; }
        public string OwnerType => Employee_Id.HasValue ? "Employee" : "Applicant";
        public int Applicant_Id { get; set; }
        public int? Employee_Id { get; set; }
    }
}
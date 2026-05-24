namespace Api.Contracts.Document
{
    public class DocumentCreateDto
    {
        public int? Employee_ID { get; set; }
        public int? Applicant_ID { get; set; }  
        public string Doc_Type { get; set; } // e.g., "Resume", "ID", "Contract"
        public string File_Path { get; set; } // The URL or folder path where the file is stored
        public DateTime Upload_Date { get; set; } = DateTime.UtcNow;
    }
}
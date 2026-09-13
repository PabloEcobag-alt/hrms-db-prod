namespace Api.Contracts.Employee
{
    public class ApplicantDto
    {
        public int id { get; set; }
        public string name { get; set; } // Computed: First_Name + Last_Name
        public string initials { get; set; } // Computed
        public string position { get; set; }
        public string status { get; set; }
        public string stage { get; set; }
        public string source { get; set; }
        public ApplicantDocumentsDto documents { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public int avatarIndex { get; set; }
        public string appliedDate { get; set; } // ISO format: YYYY-MM-DD
        public string interviewDate { get; set; } // ISO format: YYYY-MM-DD
        public string expectedStart { get; set; } // ISO format: YYYY-MM-DD
    }

    public class ApplicantDocumentsDto
    {
        public bool nbi { get; set; }
        public bool medical { get; set; }
        public bool xray { get; set; }
    }
}

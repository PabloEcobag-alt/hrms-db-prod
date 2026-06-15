namespace Api.Contracts.Employee
{
    public class EmployeeDto
    {
        public int id { get; set; }
        public string name { get; set; } // Computed: First_Name + Last_Name
        public string initials { get; set; } // Computed
        public string position { get; set; }
        public string department { get; set; }
        public string hireDate { get; set; } // ISO format: YYYY-MM-DD
        public string status { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int avatarIndex { get; set; }
        public DocumentStatusDto documents { get; set; }
        public string address { get; set; }
        public EmergencyContactDto emergencyContact { get; set; }
        public string dateOfBirth { get; set; } // ISO format: YYYY-MM-DD
        public string gender { get; set; }
        public string civilStatus { get; set; }
        public string bloodType { get; set; }
        public GovernmentIdsDto governmentIds { get; set; }
        public CompanyPropertyDto companyProperty { get; set; }
        public AttendanceSummaryDto attendanceSummary { get; set; }
    }

    public class DocumentStatusDto
    {
        public DocumentStatusItemDto personal { get; set; }
        public DocumentStatusItemDto government { get; set; }
        public DocumentStatusItemDto company { get; set; }
        public DocumentStatusItemDto performance { get; set; }
    }

    public class DocumentStatusItemDto
    {
        public bool completed { get; set; }
        public string lastUpdated { get; set; } // ISO format: YYYY-MM-DD
        public string? expiryDate { get; set; } // ISO format: YYYY-MM-DD
    }

    public class EmergencyContactDto
    {
        public string name { get; set; }
        public string relationship { get; set; }
        public string phone { get; set; }
    }

    public class GovernmentIdsDto
    {
        public string sss { get; set; }
        public string philHealth { get; set; }
        public string tin { get; set; }
        public string hdmf { get; set; }
    }

    public class CompanyPropertyDto
    {
        public string employeeId { get; set; } // e.g., "EMP-001"
        public string idIssueDate { get; set; } // ISO format: YYYY-MM-DD
        public UniformSizeDto uniformSize { get; set; }
        public string uniformIssueDate { get; set; } // ISO format: YYYY-MM-DD
        public string[] equipment { get; set; }
        public string? returnDate { get; set; } // ISO format: YYYY-MM-DD
    }

    public class UniformSizeDto
    {
        public string top { get; set; }
        public string bottom { get; set; }
        public string shoes { get; set; }
    }

    public class AttendanceSummaryDto
    {
        public int attendanceRate { get; set; }
        public int tardinessCount { get; set; }
        public int leaveBalance { get; set; }
        public int totalDaysWorked { get; set; }
    }
}

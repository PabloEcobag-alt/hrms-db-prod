using Microsoft.AspNetCore.Http;

namespace Api.Contracts.Digital201
{
    public class CreateEmployeeDto
    {
        // Step 1: Core Profile & Personal Info
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime DateHired { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Supervisor { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string EmergencyContactRelationship { get; set; } = string.Empty;

        // Step 1: Initial Documents (Files)
        public IFormFile? ResumeFile { get; set; }
        public IFormFile? PersonalDataSheetFile { get; set; }
        public IFormFile? IdPictureFile { get; set; }
        public IFormFile? BirthCertificateFile { get; set; }
        public IFormFile? MarriageCertificateFile { get; set; }

        // Step 2: Government Requirements
        public string SSS { get; set; } = string.Empty;
        public string PhilHealth { get; set; } = string.Empty;
        public string PagIbig { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;
        public DateTime? NBIExpirationDate { get; set; }
        public DateTime? BarangayExpirationDate { get; set; }

        // Step 3: Employment Documents (Files)
        public IFormFile? JobDescriptionFile { get; set; }
        public IFormFile? EmploymentContractFile { get; set; }
        public IFormFile? CompanyRulesFile { get; set; }
        public IFormFile? NDAFile { get; set; }
        public IFormFile? HandbookFile { get; set; }

        // Step 3: Payroll & Compensation
        public string BankDetails { get; set; } = string.Empty;

        // Step 3: Payroll Documents (Files)
        public IFormFile? SalaryAgreementFile { get; set; }
        public IFormFile? BIR2316File { get; set; }
        public IFormFile? AttendanceRecordFile { get; set; }

        // Step 3: Company Property Tracking
        public bool UniformIssued { get; set; }
        public bool CompanyIdIssued { get; set; }
        public string EquipmentIssued { get; set; } = string.Empty;

        // Step 3: Property Documents (Files)
        public IFormFile? AcknowledgmentReceiptFile { get; set; }

        // Step 4: Health Documents (Files)
        public IFormFile? MedicalCertificateFile { get; set; }
        public IFormFile? DrugTestFile { get; set; }
        public IFormFile? VaccinationCardFile { get; set; }

        // Step 4: Performance Documents (Files)
        public IFormFile? PerformanceEvaluationFile { get; set; }
        public IFormFile? IncidentReportFile { get; set; }
        public IFormFile? DisciplinaryRecordFile { get; set; }
        public IFormFile? PromotionRecordFile { get; set; }

        // Step 4: Admin Sign-Off
        public string CheckedBy { get; set; } = string.Empty;
        public DateTime? CheckedDate { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}

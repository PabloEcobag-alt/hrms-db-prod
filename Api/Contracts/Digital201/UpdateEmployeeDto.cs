using Microsoft.AspNetCore.Http;

namespace Api.Contracts.Digital201
{
    public class UpdateEmployeeDto
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? DateHired { get; set; }
        public string? Status { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Position { get; set; }
        public string? AssignedLocation { get; set; }
        public string? Department { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactAddress { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? SSS { get; set; }
        public string? PhilHealth { get; set; }
        public string? PagIbig { get; set; }
        public string? TIN { get; set; }
        public string? NbiClearanceDate { get; set; }
        public string? BarangayClearanceDate { get; set; }
        public string? PoliceClearanceDate { get; set; }
        public string? MedicalClearanceDate { get; set; }
        public string? BrgyIdNumber { get; set; }
        public string? BrgyIdIssueDate { get; set; }
        public string? CompanyIdNumber { get; set; }
        public string? CompanyIdIssueDate { get; set; }
        public bool? CompanyIdIssued { get; set; }
        public string? UniformTop { get; set; }
        public string? UniformBottom { get; set; }
        public string? UniformShoes { get; set; }
        public string? CheckedBy { get; set; }
        public string? CheckedDate { get; set; }
        public string? Remarks { get; set; }

        // Document Files (optional)
        public IFormFile? ResumeFile { get; set; }
        public IFormFile? PersonalDataSheetFile { get; set; }
        public IFormFile? IdPictureFile { get; set; }
        public IFormFile? BirthCertificateFile { get; set; }
        public IFormFile? MarriageCertificateFile { get; set; }
        public IFormFile? JobDescriptionFile { get; set; }
        public IFormFile? EmploymentContractFile { get; set; }
        public IFormFile? CompanyRulesFile { get; set; }
        public IFormFile? NdaFile { get; set; }
        public IFormFile? HandbookFile { get; set; }
        public IFormFile? SalaryAgreementFile { get; set; }
        public IFormFile? Bir2316File { get; set; }
        public IFormFile? AttendanceRecordFile { get; set; }
        public IFormFile? AcknowledgmentReceiptFile { get; set; }
        public IFormFile? MedicalCertificateFile { get; set; }
        public IFormFile? DrugTestFile { get; set; }
        public IFormFile? VaccinationCardFile { get; set; }
        public IFormFile? PerformanceEvaluationFile { get; set; }
        public IFormFile? IncidentReportFile { get; set; }
        public IFormFile? DisciplinaryRecordFile { get; set; }
        public IFormFile? PromotionRecordFile { get; set; }
    }
}

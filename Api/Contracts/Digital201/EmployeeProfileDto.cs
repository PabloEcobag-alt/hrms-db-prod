namespace Api.Contracts.Digital201
{
    public class EmployeeProfileDto
    {
        public int EmployeeId { get; set; }
        public string? ErpUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string DateHired { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string AssignedLocation { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Supervisor { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string EmergencyContactAddress { get; set; } = string.Empty;
        public string EmergencyContactRelationship { get; set; } = string.Empty;
        public string SSS { get; set; } = string.Empty;
        public string PhilHealth { get; set; } = string.Empty;
        public string PagIbig { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;
        public string NbiClearanceDate { get; set; } = string.Empty;
        public string BarangayClearanceDate { get; set; } = string.Empty;
        public string PoliceClearanceDate { get; set; } = string.Empty;
        public string MedicalClearanceDate { get; set; } = string.Empty;
        public string BrgyIdNumber { get; set; } = string.Empty;
        public string BrgyIdIssueDate { get; set; } = string.Empty;
        public string CompanyIdNumber { get; set; } = string.Empty;
        public string CompanyIdIssueDate { get; set; } = string.Empty;
        public bool CompanyIdIssued { get; set; }
        public string BankDetails { get; set; } = string.Empty;
        public bool UniformIssued { get; set; }
        public string UniformTop { get; set; } = string.Empty;
        public string UniformBottom { get; set; } = string.Empty;
        public string UniformShoes { get; set; } = string.Empty;
        public string EquipmentIssued { get; set; } = string.Empty;
        public string CheckedBy { get; set; } = string.Empty;
        public string CheckedDate { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}

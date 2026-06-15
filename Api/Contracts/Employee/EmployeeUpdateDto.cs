using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Employee
{
    public class EmployeeUpdateDto
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? position { get; set; }
        public string? department { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? address { get; set; }
        public string? dateOfBirth { get; set; } // ISO format: YYYY-MM-DD
        public string? gender { get; set; }
        public string? civilStatus { get; set; }
        public string? bloodType { get; set; }
        public EmergencyContactDto? emergencyContact { get; set; }
        public GovernmentIdsDto? governmentIds { get; set; }
        public CompanyPropertyDto? companyProperty { get; set; }
        public string? paymentMethod { get; set; }
        public string? accountNumber { get; set; }
        public string? status { get; set; }
        public int? roleId { get; set; }
        public string? hireDate { get; set; } // ISO format: YYYY-MM-DD
    }
}
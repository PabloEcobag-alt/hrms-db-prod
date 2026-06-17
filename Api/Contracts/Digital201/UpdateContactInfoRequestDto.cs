using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api.Contracts.Digital201
{
    public class UpdateContactInfoRequestDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [StringLength(20)]
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [StringLength(500)]
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [Required]
        [JsonPropertyName("emergencyContact")]
        public EmergencyContactUpdateDto EmergencyContact { get; set; } = default!;
    }

    public class EmergencyContactUpdateDto
    {
        [Required]
        [StringLength(100)]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(100)]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = default!;

        [Required]
        [StringLength(50)]
        [JsonPropertyName("relationship")]
        public string Relationship { get; set; } = default!;

        [Required]
        [StringLength(20)]
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = default!;

        [StringLength(500)]
        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }
}

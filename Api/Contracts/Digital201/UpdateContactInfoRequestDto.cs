using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Digital201
{
    public class UpdateContactInfoRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [Required]
        public EmergencyContactUpdateDto EmergencyContact { get; set; } = default!;
    }

    public class EmergencyContactUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = default!;

        [Required]
        [StringLength(50)]
        public string Relationship { get; set; } = default!;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = default!;

        [StringLength(500)]
        public string? Address { get; set; }
    }
}

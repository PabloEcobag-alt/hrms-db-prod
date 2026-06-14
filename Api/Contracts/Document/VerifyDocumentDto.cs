using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Document
{
    public class VerifyDocumentDto
    {
        [Required]
        public bool IsApproved { get; set; }

        public string? RejectionReason { get; set; }
    }
}

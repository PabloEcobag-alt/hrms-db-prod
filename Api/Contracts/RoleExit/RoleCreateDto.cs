using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.RoleExit
{
    public class RoleCreateDto
    {
        [Required]
        public string Role_Id { get; set; } = default!;
        public string Role_Description { get; set; } = default!;
    }
}
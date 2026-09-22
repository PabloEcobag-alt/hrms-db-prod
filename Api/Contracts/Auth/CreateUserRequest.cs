using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Auth
{
    public class CreateUserRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public List<AppPermissionDto> Apps { get; set; } = new();
    }

    public class AppPermissionDto
    {
        [Required]
        public string AppName { get; set; } = string.Empty;

        public List<ModulePermissionDto> Modules { get; set; } = new();
    }

    public class ModulePermissionDto
    {
        [Required]
        public string ModuleName { get; set; } = string.Empty;
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanApprove { get; set; }
        public bool CanExport { get; set; }
    }
}

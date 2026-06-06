using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.RoleExit
{
    public class RoleCreateDto
    {
        public string Role_Description { get; set; } = default!;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.RoleExit;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleReadDto>>> GetAll()
        {
            return Ok(await _roleService.GetAllRolesAsync());
        }

        [HttpPost]
        public async Task<ActionResult<RoleReadDto>> Create(RoleCreateDto createDto)
        {
            var role = await _roleService.CreateRoleAsync(createDto);
            return CreatedAtAction(nameof(GetAll), new { id = role.Role_Id }, role);
        }
    }
}
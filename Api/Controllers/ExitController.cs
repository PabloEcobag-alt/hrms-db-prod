using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.RoleExit;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ExitCanRead")]
    public class ExitsController : ControllerBase
    {
        private readonly IExitService _exitService;

        public ExitsController(IExitService exitService)
        {
            _exitService = exitService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExitReadDto>>> GetAll()
        {
            return Ok(await _exitService.GetAllExitsAsync());
        }

        [HttpPost]
        public async Task<ActionResult<ExitReadDto>> Create(ExitCreateDto createDto)
        {
            var result = await _exitService.CreateExitRecordAsync(createDto);
            return Ok(result);
        }
    }
}
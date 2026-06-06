using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.Checklist;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChecklistsController : ControllerBase
    {
        private readonly IChecklistService _checklistService;

        public ChecklistsController(IChecklistService checklistService)
        {
            _checklistService = checklistService;
        }

        [HttpGet("applicant/{applicantId}")]
        public async Task<ActionResult<IEnumerable<ChecklistReadDto>>> GetByApplicant(int applicantId)
        {
            var checklist = await _checklistService.GetChecklistByApplicantIdAsync(applicantId);
            return Ok(checklist);
        }

        [HttpPut]
        public async Task<ActionResult> Update(ChecklistUpdateDto updateDto)
        {
            var success = await _checklistService.UpdateChecklistAsync(updateDto);
            if (!success) return BadRequest("Could not update checklist");
            return NoContent();
        }
    }
}
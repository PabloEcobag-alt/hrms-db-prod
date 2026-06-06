using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.Applicant;
using Api.Contracts.Employee;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicantsController : ControllerBase
    {
        private readonly IApplicantService _applicantService;

        public ApplicantsController(IApplicantService applicantService)
        {
            _applicantService = applicantService;
        }

        [HttpPost]
        public async Task<ActionResult<ApplicantReadDto>> Create(ApplicantCreateDto createDto)
        {
            var applicant = await _applicantService.CreateApplicantAsync(createDto);
            return CreatedAtAction(nameof(GetAll), new { id = applicant.Applicant_Id }, applicant);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicantReadDto>>> GetAll()
        {
            return Ok(await _applicantService.GetAllApplicantsAsync());
        }

        [HttpPost("hire")] // URL is now: api/applicants/hire
        public async Task<ActionResult<EmployeeReadDto>> Hire([FromBody] HireRequest request)
        {
            var employee = await _applicantService.HireApplicantAsync(request.ApplicantId, request.RoleId);
            
            if (employee == null) return NotFound("Applicant not found");

            return Ok(employee);
        }
    }
}
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

        [HttpPost("ecommerce-application")]
        [AllowAnonymous] // Allow public access from web-ecommerce
        public async Task<ActionResult<ApplicantReadDto>> CreateEcommerceApplication([FromBody] EcommerceApplicationDto dto)
        {
            var applicant = await _applicantService.CreateEcommerceApplicationAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = applicant.Applicant_Id }, applicant);
        }

        [HttpGet("recent")]
        public async Task<ActionResult<ApplicantReadDto>> GetRecentEcommerceApplicant()
        {
            var applicant = await _applicantService.GetRecentEcommerceApplicantAsync();
            
            if (applicant == null) return NotFound("No recent ecommerce applicant found");

            return Ok(applicant);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ApplicantReadDto>> Update(int id, [FromBody] ApplicantUpdateDto updateDto)
        {
            var applicant = await _applicantService.UpdateApplicantAsync(id, updateDto);
            
            if (applicant == null) return NotFound("Applicant not found");

            return Ok(applicant);
        }
    }
}
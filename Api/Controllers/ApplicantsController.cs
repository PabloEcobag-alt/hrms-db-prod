using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Applications.Interfaces;
using Applications.Commands;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Api.Contracts.Applicant;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicantsController : ControllerBase
    {
        private readonly IApplicantService _applicantService;
        private readonly IMediator _mediator;

        public ApplicantsController(IApplicantService applicantService, IMediator mediator)
        {
            _applicantService = applicantService;
            _mediator = mediator;
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
        public async Task<ActionResult<EmployeeReadDto>> Hire([FromBody] HireApplicantDto dto)
        {
            var command = new HireApplicantCommand
            {
                ApplicantId = dto.ApplicantId,
                StartDate = dto.StartDate,
                ProbationaryEndDate = dto.ProbationaryEndDate,
                HiringStage = dto.HiringStage
            };

            var employee = await _mediator.Send(command);
            
            if (employee == null) return NotFound("Applicant not found");

            return Ok(employee);
        }

        [HttpPost("ecommerce-application")]
        [AllowAnonymous] // Allow public access from web-ecommerce
        public async Task<ActionResult<ApplicantReadDto>> CreateEcommerceApplication([FromForm] EcommerceApplicationDto dto)
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

        [HttpPost("{id}/transform")]
        public async Task<ActionResult<EmployeeReadDto>> Transform(int id)
        {
            // Use default values for transformation (can be configured later)
            var command = new HireApplicantCommand
            {
                ApplicantId = id,
                StartDate = DateTime.UtcNow,
                ProbationaryEndDate = null,
                HiringStage = "Regular"
            };
            
            var employee = await _mediator.Send(command);
            
            if (employee == null) return NotFound("Applicant not found");

            return Ok(employee);
        }
    }
}
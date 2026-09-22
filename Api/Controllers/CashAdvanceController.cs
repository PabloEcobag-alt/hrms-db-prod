using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Contracts.CashAdvance;
using Applications.Exceptions;
using Applications.Interfaces;

namespace ApiHrm.Controllers
{
    [Route("api/cash-advances")]
    [ApiController]
    [Authorize(Policy = "CashAdvanceCanRead")]
    public class CashAdvanceController : ControllerBase
    {
        private readonly ICashAdvanceService _cashAdvanceService;

        public CashAdvanceController(ICashAdvanceService cashAdvanceService)
        {
            _cashAdvanceService = cashAdvanceService;
        }

        /// <summary>
        /// Submit a cash advance request. Blocks requests that exceed the employee's
        /// earned value for the current cutoff period (days worked × daily rate).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CashAdvanceReadDto>> Create(
            [FromBody] CashAdvanceCreateDto dto)
        {
            try
            {
                var result = await _cashAdvanceService.CreateAsync(dto);
                return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all cash advance requests for Admin/Manager review.
        /// Restricted to HR and Manager roles.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "CashAdvanceCanApprove")]
        public async Task<ActionResult<IEnumerable<CashAdvanceReadDto>>> GetAll()
        {
            try
            {
                var result = await _cashAdvanceService.GetAllAsync();
                return Ok(result);
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

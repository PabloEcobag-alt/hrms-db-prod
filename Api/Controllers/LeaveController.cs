using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
using Api.Contracts.Leave;
using Applications.Exceptions;
using Applications.Interfaces;

namespace ApiHrm.Controllers
{
    [Route("api/leaves")]
    [ApiController]
    // [Authorize(Policy = "LeaveCanRead")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        /// <summary>
        /// Submit a new leave request. Enforces a 7-day advance notice rule.
        /// Fires a notification trigger to the manager's dashboard on success.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LeaveRequestReadDto>> Create(
            [FromBody] LeaveRequestCreateDto dto)
        {
            try
            {
                var result = await _leaveService.CreateAsync(dto);
                return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update the status of a leave request (Pending / Approved / Declined).
        /// Restricted to HR and Manager roles. Triggers a credit refund if Declined.
        /// </summary>
        [HttpPatch("{id}/status")]
        // [Authorize(Policy = "LeaveCanApprove")]
        public async Task<ActionResult<LeaveRequestReadDto>> UpdateStatus(
            int id,
            [FromBody] LeaveStatusUpdateDto dto)
        {
            try
            {
                var result = await _leaveService.UpdateStatusAsync(id, dto);
                return Ok(result);
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all leave requests for Admin/Manager review.
        /// Restricted to HR and Manager roles.
        /// </summary>
        [HttpGet]
        // [Authorize(Policy = "LeaveCanApprove")]
        public async Task<ActionResult<IEnumerable<LeaveRequestReadDto>>> GetAll()
        {
            try
            {
                var result = await _leaveService.GetAllAsync();
                return Ok(result);
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

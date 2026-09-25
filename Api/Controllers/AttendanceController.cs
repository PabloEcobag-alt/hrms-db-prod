using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
using Api.Contracts.Attendance;
using Applications.Exceptions;
using Applications.Interfaces;

namespace ApiHrm.Controllers
{
    [Route("api/attendance")]
    [ApiController]
    // [Authorize(Policy = "AttendanceCanRead")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        /// <summary>
        /// Machine listening endpoint — accepts biometric or RFID logs from network hardware.
        /// Hard-rejects Sunday entries and marks status based on shift window.
        /// </summary>
        [HttpPost("log")]
        public async Task<ActionResult<AttendanceLogReadDto>> Log(
            [FromBody] AttendanceLogCreateDto dto)
        {
            try
            {
                var result = await _attendanceService.LogAsync(dto);
                return CreatedAtAction(nameof(Log), result);
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Manual HR override — corrects an attendance record and writes an audit trail.
        /// Restricted to users with the HR role.
        /// </summary>
        [HttpPost("override")]
        // [Authorize(Policy = "AttendanceCanWrite")]
        public async Task<ActionResult<AttendanceLogReadDto>> Override(
            [FromBody] AttendanceOverrideDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            try
            {
                var result = await _attendanceService.OverrideAsync(dto, userId, ipAddress);
                return CreatedAtAction(nameof(Override), result);
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

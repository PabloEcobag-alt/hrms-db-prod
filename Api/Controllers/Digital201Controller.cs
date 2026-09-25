using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Applications.Interfaces;
using Api.Contracts.Digital201;

namespace ApiHrm.Controllers
{
    [Route("api/digital201")]
    [ApiController]
    // [Authorize(Policy = "Digital201CanRead")]
    public class Digital201Controller : ControllerBase
    {
        private readonly IDigital201Service _digital201Service;
        private readonly ILogger<Digital201Controller> _logger;

        public Digital201Controller(IDigital201Service digital201Service, ILogger<Digital201Controller> logger)
        {
            _digital201Service = digital201Service;
            _logger = logger;
        }

        [HttpGet("contact-info")]
        public async Task<ActionResult<GetContactInfoResponseDto>> GetContactInfo()
        {
            try
            {
                // Extract ErpUserId from JWT claims
                var erpUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(erpUserId))
                {
                    _logger.LogWarning("Unauthorized access attempt - missing ErpUserId in JWT claims");
                    return Unauthorized("Invalid user token");
                }

                _logger.LogInformation("Processing contact info get request for ErpUserId: {ErpUserId}", erpUserId);

                var contactInfo = await _digital201Service.GetContactInfoAsync(erpUserId);

                if (contactInfo == null)
                {
                    _logger.LogWarning("Contact info not found for ErpUserId: {ErpUserId}", erpUserId);
                    return NotFound("Contact information not found");
                }

                _logger.LogInformation("Successfully retrieved contact info for ErpUserId: {ErpUserId}", erpUserId);
                return Ok(contactInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact info get request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("contact-info")]
        public async Task<ActionResult> UpdateContactInfo([FromBody] UpdateContactInfoRequestDto request)
        {
            try
            {
                // Extract ErpUserId from JWT claims
                var erpUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(erpUserId))
                {
                    _logger.LogWarning("Unauthorized access attempt - missing ErpUserId in JWT claims");
                    return Unauthorized("Invalid user token");
                }

                _logger.LogInformation("Processing contact info update request for ErpUserId: {ErpUserId}", erpUserId);

                var success = await _digital201Service.UpdateContactInfoAsync(erpUserId, request);

                if (success)
                {
                    _logger.LogInformation("Successfully updated contact info for ErpUserId: {ErpUserId}", erpUserId);
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning("Failed to update contact info for ErpUserId: {ErpUserId}", erpUserId);
                    return BadRequest("Failed to update contact information");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact info update request");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

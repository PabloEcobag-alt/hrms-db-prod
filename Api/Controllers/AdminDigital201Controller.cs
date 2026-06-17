using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using Api.Contracts.Digital201;

namespace ApiHrm.Controllers
{
    [ApiController]
    [Route("api/admin/digital201")]
    [Authorize]
    public class AdminDigital201Controller : ControllerBase
    {
        private readonly IDigital201Service _digital201Service;
        private readonly ILogger<AdminDigital201Controller> _logger;

        public AdminDigital201Controller(IDigital201Service digital201Service, ILogger<AdminDigital201Controller> logger)
        {
            _digital201Service = digital201Service;
            _logger = logger;
        }

        [HttpGet("employees")]
        public async Task<ActionResult<List<AdminEmployeeListDto>>> GetAllEmployees()
        {
            try
            {
                _logger.LogInformation("Processing request to get all employees");

                var employees = await _digital201Service.GetAllEmployeesAsync();

                _logger.LogInformation("Successfully retrieved {Count} employees", employees.Count);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to get all employees");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("employees")]
        public async Task<ActionResult<int>> CreateEmployee([FromForm] CreateEmployeeDto dto)
        {
            try
            {
                _logger.LogInformation("Processing request to create new employee: {FirstName} {LastName}", dto.FirstName, dto.LastName);

                var employeeId = await _digital201Service.CreateEmployeeAsync(dto);

                _logger.LogInformation("Successfully created employee with ID: {EmployeeId}", employeeId);
                return CreatedAtAction(nameof(GetAllEmployees), new { }, employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to create employee");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("employees/check-duplicate")]
        public async Task<ActionResult<bool>> CheckDuplicateEmployee([FromQuery] string firstName, [FromQuery] string lastName)
        {
            try
            {
                var isDuplicate = await _digital201Service.CheckDuplicateEmployeeAsync(firstName, lastName);
                return Ok(isDuplicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking duplicate employee for {FirstName} {LastName}", firstName, lastName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("documents/expiring")]
        public async Task<ActionResult<List<ExpiringDocumentDto>>> GetExpiringDocuments([FromQuery] int days = 30)
        {
            try
            {
                var expiringDocuments = await _digital201Service.GetExpiringDocumentsAsync(days);
                return Ok(expiringDocuments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expiring documents");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult<EmployeeProfileDto>> GetEmployeeProfile(int id)
        {
            try
            {
                var profile = await _digital201Service.GetEmployeeProfileAsync(id);
                if (profile == null)
                {
                    return NotFound();
                }
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee profile for ID: {EmployeeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-erp-user/{erpUserId}")]
        public async Task<ActionResult<EmployeeProfileDto>> GetEmployeeByErpUserId(string erpUserId)
        {
            try
            {
                var profile = await _digital201Service.GetEmployeeByErpUserIdAsync(erpUserId);
                if (profile == null)
                {
                    return NotFound();
                }
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee profile for ErpUserId: {ErpUserId}", erpUserId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("employees/{id}")]
        [Route("employees/{id}")]
        public async Task<ActionResult<bool>> UpdateEmployee(int id, [FromForm] UpdateEmployeeDto dto)
        {
            try
            {
                _logger.LogInformation("Processing request to update employee with ID: {EmployeeId}", id);

                var result = await _digital201Service.UpdateEmployeeAsync(id, dto);

                if (result)
                {
                    _logger.LogInformation("Successfully updated employee with ID: {EmployeeId}", id);
                    return Ok(true);
                }
                else
                {
                    _logger.LogWarning("Failed to update employee with ID: {EmployeeId}", id);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to update employee with ID: {EmployeeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("employees/unregistered")]
        public async Task<ActionResult<List<UnregisteredEmployeeDto>>> GetUnregisteredEmployees()
        {
            try
            {
                _logger.LogInformation("Processing request to get unregistered employees");

                var employees = await _digital201Service.GetUnregisteredEmployeesAsync();

                _logger.LogInformation("Successfully retrieved {Count} unregistered employees", employees.Count);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to get unregistered employees");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("employees/{employeeId}/erp-user-id")]
        public async Task<ActionResult<bool>> UpdateEmployeeErpUserId(int employeeId, [FromBody] UpdateErpUserIdRequest request)
        {
            try
            {
                _logger.LogInformation("Processing request to update ErpUserId for employee {EmployeeId}", employeeId);

                var result = await _digital201Service.UpdateEmployeeErpUserIdAsync(employeeId, request.ErpUserId);

                if (result)
                {
                    _logger.LogInformation("Successfully updated ErpUserId for employee {EmployeeId}", employeeId);
                    return Ok(true);
                }
                else
                {
                    _logger.LogWarning("Failed to update ErpUserId for employee {EmployeeId}", employeeId);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to update ErpUserId for employee {EmployeeId}", employeeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("employees/link-erp-user")]
        public async Task<ActionResult<bool>> LinkErpUser([FromBody] LinkErpUserRequest request)
        {
            try
            {
                _logger.LogInformation("Processing request to link ErpUserId {ErpUserId} to employee {EmployeeId}", request.ErpUserId, request.EmployeeId);

                var result = await _digital201Service.UpdateEmployeeErpUserIdAsync(request.EmployeeId, request.ErpUserId);

                if (result)
                {
                    _logger.LogInformation("Successfully linked ErpUserId {ErpUserId} to employee {EmployeeId}", request.ErpUserId, request.EmployeeId);
                    return Ok(true);
                }
                else
                {
                    _logger.LogWarning("Failed to link ErpUserId {ErpUserId} to employee {EmployeeId}", request.ErpUserId, request.EmployeeId);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to link ErpUserId {ErpUserId} to employee {EmployeeId}", request.ErpUserId, request.EmployeeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("contact-info")]
        public async Task<ActionResult<object>> GetContactInfo()
        {
            try
            {
                // Get ErpUserId from current user's token claims
                var erpUserIdClaim = User.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(erpUserIdClaim))
                {
                    return Unauthorized("ErpUserId not found in token");
                }

                _logger.LogInformation("Processing contact info get request for ErpUserId: {ErpUserId}", erpUserIdClaim);

                var contactInfo = await _digital201Service.GetContactInfoAsync(erpUserIdClaim);

                if (contactInfo == null)
                {
                    _logger.LogWarning("Contact info not found for ErpUserId: {ErpUserId}", erpUserIdClaim);
                    return NotFound();
                }

                _logger.LogInformation("Successfully retrieved contact info for ErpUserId: {ErpUserId}", erpUserIdClaim);
                return Ok(contactInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact info request");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

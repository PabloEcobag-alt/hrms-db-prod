using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using Api.Contracts.Digital201;

namespace ApiHrm.Controllers
{
    [ApiController]
    [Route("api/admin/digital201")]
    [Authorize(Policy = "AdminDigital201CanRead")]
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

        [HttpGet("employees/by-erp-user/{erpUserId}")]
        public async Task<ActionResult<EmployeeProfileDto>> GetEmployeeByErpUserId(string erpUserId)
        {
            try
            {
                _logger.LogInformation("Processing request to get employee by ErpUserId: {ErpUserId}", erpUserId);

                var employee = await _digital201Service.GetEmployeeByErpUserIdAsync(erpUserId);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found for ErpUserId: {ErpUserId}", erpUserId);
                    return NotFound("Employee not found for ErpUserId");
                }

                _logger.LogInformation("Successfully retrieved employee for ErpUserId: {ErpUserId}", erpUserId);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to get employee by ErpUserId");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("documents/expiring")]
        public async Task<ActionResult<List<ExpiringDocumentDto>>> GetExpiringDocuments([FromQuery] int days = 30)
        {
            try
            {
                _logger.LogInformation("Processing request to get expiring documents within {Days} days", days);

                var documents = await _digital201Service.GetExpiringDocumentsAsync(days);

                _logger.LogInformation("Successfully retrieved {Count} expiring documents", documents.Count);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to get expiring documents");
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

        [HttpGet("employees/check-duplicate")]
        public async Task<ActionResult<bool>> CheckDuplicateEmployee([FromQuery] string firstName, [FromQuery] string lastName)
        {
            try
            {
                _logger.LogInformation("Processing request to check duplicate employee: {FirstName} {LastName}", firstName, lastName);

                var isDuplicate = await _digital201Service.CheckDuplicateEmployeeAsync(firstName, lastName);

                _logger.LogInformation("Duplicate check result for {FirstName} {LastName}: {IsDuplicate}", firstName, lastName, isDuplicate);
                return Ok(isDuplicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to check duplicate employee");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult<EmployeeProfileDto>> GetEmployeeById(int id)
        {
            try
            {
                _logger.LogInformation("Processing request to get employee by ID: {EmployeeId}", id);

                var employee = await _digital201Service.GetEmployeeProfileAsync(id);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found for ID: {EmployeeId}", id);
                    return NotFound("Employee not found");
                }

                _logger.LogInformation("Successfully retrieved employee for ID: {EmployeeId}", id);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to get employee by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("employees/{id}")]
        public async Task<ActionResult<bool>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            try
            {
                _logger.LogInformation("Processing request to update employee with ID: {EmployeeId}", id);

                var result = await _digital201Service.UpdateEmployeeAsync(id, dto);

                if (!result)
                {
                    _logger.LogWarning("Failed to update employee with ID: {EmployeeId}", id);
                    return NotFound("Employee not found or update failed");
                }

                _logger.LogInformation("Successfully updated employee with ID: {EmployeeId}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to update employee");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("employees/link-erp-user")]
        public async Task<ActionResult> LinkErpUser([FromBody] LinkErpUserDto dto)
        {
            try
            {
                _logger.LogInformation("Processing request to link ERP user {ErpUserId} to employee {EmployeeId}", dto.ErpUserId, dto.EmployeeId);

                var result = await _digital201Service.UpdateEmployeeErpUserIdAsync(dto.EmployeeId, dto.ErpUserId);

                if (!result)
                {
                    _logger.LogWarning("Failed to link ERP user to employee {EmployeeId}", dto.EmployeeId);
                    return NotFound("Employee not found");
                }

                _logger.LogInformation("Successfully linked ERP user {ErpUserId} to employee {EmployeeId}", dto.ErpUserId, dto.EmployeeId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to link ERP user");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

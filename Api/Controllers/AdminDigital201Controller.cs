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
    }
}

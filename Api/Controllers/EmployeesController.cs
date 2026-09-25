using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Policy = "EmployeeInformationCanRead")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById([FromRoute]int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            
            return Ok(employee);
        }

        [HttpPost]
        // [Authorize(Policy = "EmployeeInformationCanWrite")]
        public async Task<ActionResult<EmployeeDto>> Create(EmployeeCreateDto createDto)
        {
            var result = await _employeeService.CreateEmployeeAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = result.id }, result);
        }

        [HttpPut("{id}")]
        // [Authorize(Policy = "EmployeeInformationCanUpdate")]
        public async Task<ActionResult> Update(int id, EmployeeUpdateDto updateDto)
        {
            var success = await _employeeService.UpdateEmployeeProfileAsync(id, updateDto);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
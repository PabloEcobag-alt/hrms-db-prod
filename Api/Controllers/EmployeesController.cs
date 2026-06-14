using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDetailDto>> GetById([FromRoute]int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            
            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles = "HR,HRAdmin")]
        public async Task<ActionResult<EmployeeReadDto>> Create(EmployeeCreateDto createDto)
        {
            var result = await _employeeService.CreateEmployeeAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Employee_Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HR,HRAdmin")]
        public async Task<ActionResult> Update(int id, EmployeeUpdateDto updateDto)
        {
            var success = await _employeeService.UpdateEmployeeAsync(id, updateDto);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
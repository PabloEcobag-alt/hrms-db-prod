using Microsoft.AspNetCore.Mvc;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.Employee;
using Applications.Exceptions;
using System.Security.Claims;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<EmployeeReadDto>> Create(EmployeeCreateDto createDto)
        {
            try
            {
                var result = await _employeeService.CreateEmployeeAsync(createDto, GetChangedBy());
                return CreatedAtAction(nameof(GetById), new { id = result.Employee_Id }, result);
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, EmployeeUpdateDto updateDto)
        {
            try
            {
                var updated = await _employeeService.UpdateEmployeeAsync(id, updateDto, GetChangedBy());
                if (!updated) return NotFound();

                return NoContent();
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetChangedBy()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(ClaimTypes.Email)
                ?? "System";
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Api.Contracts.LeaveType;
using AutoMapper;
using ApiHrm.Infrastructures.Persistence;

namespace ApiHrm.Controllers
{
    [Route("api/leave-types")]
    [ApiController]
    [Authorize]
    public class LeaveTypeController : ControllerBase
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        public LeaveTypeController(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all available leave types (e.g., VL, SL).
        /// Used by the ESS leave request form to populate the leave type dropdown.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveTypeReadDto>>> GetAll()
        {
            var types = await _context.LeaveTypes.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<LeaveTypeReadDto>>(types));
        }
    }
}

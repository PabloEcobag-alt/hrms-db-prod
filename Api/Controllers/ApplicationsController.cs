using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
using ApiHrm.Api.Contracts.Applicant;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [AllowAnonymous] // Allow public access since it's an external form
    public class ApplicationsController : ControllerBase
    {
        private readonly hrmAppDbContext _context;

        public ApplicationsController(hrmAppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] OnlineShopApplicationDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid payload.");
            }

            // Name Split
            var nameParts = dto.Name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string firstName = nameParts.Length > 0 ? nameParts[0] : "";
            string lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : ".";
            
            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = "Unknown";
            }

            // Phone Normalization
            string mobile = dto.Phone.Replace(" ", "").Trim();
            if (mobile.StartsWith("09"))
            {
                mobile = "+639" + mobile.Substring(2);
            }

            var applicant = new Applicant
            {
                First_Name = firstName,
                Last_Name = lastName,
                Email = dto.Email,
                Mobile = mobile,
                Position = dto.JobPostingId, // Map JobPostingId to Position
                Hiring_Stage = "Applied",
                Source = "Website Portal", // Good default for online-shop
                Status = "Training", // Based on Expected: "Training" | "Probationary" | "Regular"
                Application_Date = DateOnly.FromDateTime(DateTime.UtcNow)
                // Resume_URL could be handled by a file upload service if needed, currently leaving empty or we can add logic to save it
            };

            _context.Applicants.Add(applicant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Application submitted successfully" });
        }
    }
}

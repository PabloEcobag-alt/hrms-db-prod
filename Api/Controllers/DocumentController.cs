using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.Document;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "DocumentCanRead")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // Updated to match GetDocumentsByOwnerAsync
        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<DocumentReadDto>>> GetByOwner(int ownerId, [FromQuery] string ownerType = "Employee")
        {
            var documents = await _documentService.GetDocumentsByOwnerAsync(ownerId, ownerType);
            return Ok(documents);
        }

        // Updated to use DocumentUploadDto as required by your interface
        [HttpPost]
        [Authorize(Policy = "DocumentCanWrite")]
        public async Task<ActionResult<DocumentReadDto>> Upload(DocumentUploadDto uploadDto)
        {
            var result = await _documentService.UploadDocumentAsync(uploadDto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentReadDto>>> GetAll()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return Ok(documents);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DocumentCanDelete")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _documentService.DeleteDocumentAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/verify")]
        [Authorize(Policy = "DocumentCanApprove")]
        public async Task<ActionResult> Verify(int id, [FromBody] VerifyDocumentDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "unknown";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var success = await _documentService.VerifyDocumentAsync(id, dto, userId, role, ipAddress);
            if (!success) return NotFound();

            return Ok(new { message = "Document verification completed successfully." });
        }
    }
}
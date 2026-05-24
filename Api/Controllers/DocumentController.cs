using Microsoft.AspNetCore.Mvc;
using Applications.Interfaces;
using ApiHrm.Domains.Entities;
using Api.Contracts.Document;

namespace ApiHrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _documentService.DeleteDocumentAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
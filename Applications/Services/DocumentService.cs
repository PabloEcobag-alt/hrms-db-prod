using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence;
using ApiHrm.Domains.Entities;
using Api.Contracts.Document;
using Applications.Interfaces;

namespace Applications.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly hrmAppDbContext _context;
        private readonly IMapper _mapper;

        public DocumentService(hrmAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DocumentReadDto> UploadDocumentAsync(DocumentUploadDto uploadDto)
        {
            var document = _mapper.Map<Document>(uploadDto);
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return _mapper.Map<DocumentReadDto>(document);
        }

        public async Task<IEnumerable<DocumentReadDto>> GetAllDocumentsAsync()
        {
            var documents = await _context.Documents
                .OrderByDescending(d => d.upload_date)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DocumentReadDto>>(documents);
        }

        public async Task<IEnumerable<DocumentReadDto>> GetDocumentsByOwnerAsync(int ownerId, string ownerType)
        {
            IQueryable<Document> query = _context.Documents;

            // Logic to filter based on whether the owner is an Employee or Applicant
            if (ownerType.Equals("Employee", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(d => d.Employee_ID == ownerId);
            }
            else
            {
                query = query.Where(d => d.Applicant_ID == ownerId);
            }

            var documents = await query.ToListAsync();
            return _mapper.Map<IEnumerable<DocumentReadDto>>(documents);
        }

        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return false;

            _context.Documents.Remove(document);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
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
        private readonly IAuditLogService _auditLogService;

        public DocumentService(hrmAppDbContext context, IMapper mapper, IAuditLogService auditLogService)
        {
            _context = context;
            _mapper = mapper;
            _auditLogService = auditLogService;
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

        public async Task<bool> VerifyDocumentAsync(int documentId, VerifyDocumentDto dto, string verifierUserId, string verifierRole, string ipAddress)
        {
            var doc = await _context.EmployeeDocuments.FindAsync(documentId);
            if (doc == null) return false;

            doc.VerificationStatus = dto.IsApproved ? "Approved" : "Rejected";
            if (int.TryParse(verifierUserId, out var verId))
            {
                doc.VerifiedByUserId = verId;
            }
            else
            {
                doc.VerifiedByUserId = null;
            }
            doc.RejectionReason = dto.IsApproved ? null : dto.RejectionReason;

            await _context.SaveChangesAsync();

            var actionText = dto.IsApproved ? "verified" : "rejected";
            await _auditLogService.LogAsync(
                userId: verifierUserId,
                userRole: verifierRole,
                action: dto.IsApproved ? "VERIFY_APPROVE" : "VERIFY_REJECT",
                module: "Document",
                recordId: documentId.ToString(),
                recordLabel: $"EmployeeDocument",
                ipAddress: ipAddress
            );

            // AuditLog captures: "HR Admin [UserId] verified/rejected Document [Id] for Employee [EmpId]"
            // We can log this using an explicit message or log entry. Let's make sure our logged action detail corresponds.
            // Wait, does the audit log table have a detail/description field or is it recorded in Record_Label/Action?
            // Yes! Record_Label in AuditLog is string, we can set it to exactly:
            // "HR Admin [UserId] verified/rejected Document [Id] for Employee [EmpId]"
            var auditMsg = $"HR Admin {verifierUserId} {actionText} Document {documentId} for Employee {doc.Employee_Id}";
            
            // Let's log it under action or recordLabel:
            await _auditLogService.LogAsync(
                userId: verifierUserId,
                userRole: verifierRole,
                action: dto.IsApproved ? "APPROVE" : "REJECT",
                module: "Document",
                recordId: documentId.ToString(),
                recordLabel: auditMsg,
                ipAddress: ipAddress
            );

            return true;
        }
    }
}
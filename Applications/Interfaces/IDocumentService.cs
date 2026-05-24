using Api.Contracts.Document;

namespace Applications.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentReadDto> UploadDocumentAsync(DocumentUploadDto uploadDto);
        Task<IEnumerable<DocumentReadDto>> GetDocumentsByOwnerAsync(int ownerId, string ownerType);
        Task<IEnumerable<DocumentReadDto>> GetAllDocumentsAsync();
        Task<bool> DeleteDocumentAsync(int documentId);
    }
}
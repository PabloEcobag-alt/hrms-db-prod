namespace Applications.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(
            string userId,
            string userRole,
            string action,
            string module,
            string recordId,
            string recordLabel,
            string ipAddress);
    }
}

using ApiHrm.Domains.Entities;
using ApiHrm.Infrastructures.Persistence;
using Applications.Interfaces;

namespace Applications.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly hrmAppDbContext _context;

        public AuditLogService(hrmAppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            string userId,
            string userRole,
            string action,
            string module,
            string recordId,
            string recordLabel,
            string ipAddress)
        {
            var entry = new AuditLog
            {
                User_Id = userId,
                User_Role = userRole,
                Action = action,
                Module = module,
                Record_Id = recordId,
                Record_Label = recordLabel,
                Ip_Address = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(entry);
            await _context.SaveChangesAsync();
        }
    }
}

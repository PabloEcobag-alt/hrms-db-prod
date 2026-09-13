using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using ApiHrm.Domains.Entities;

namespace ApiHrm.Infrastructures.Persistence.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is null) return base.SavingChanges(eventData, result);

            CaptureAuditEntries(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            CaptureAuditEntries(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void CaptureAuditEntries(DbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || 
                           e.State == EntityState.Modified || 
                           e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                // Skip AuditLog entities to prevent infinite loops
                if (entry.Entity is AuditLog)
                    continue;

                var entityName = entry.Entity.GetType().Name;
                var operation = entry.State.ToString();
                string? oldValues = null;
                string? newValues = null;

                switch (entry.State)
                {
                    case EntityState.Added:
                        // Capture current values for INSERT
                        newValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                        break;

                    case EntityState.Modified:
                        // Capture both original and current values for UPDATE
                        oldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                        newValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                        break;

                    case EntityState.Deleted:
                        // Capture original values for DELETE
                        oldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                        break;
                }

                var auditLog = new AuditLog
                {
                    User_Id = "System", // TODO: Extract from user context when available
                    User_Role = "System", // TODO: Extract from user context when available
                    Action = operation,
                    Module = "HRMS", // TODO: Extract from context or categorize by entity
                    Record_Id = GetKeyValue(entry)?.ToString() ?? "Unknown",
                    Record_Label = GetRecordLabel(entry),
                    Ip_Address = "127.0.0.1", // TODO: Extract from HTTP context
                    Timestamp = DateTime.UtcNow,
                    OldValues = oldValues,
                    NewValues = newValues,
                    Operation = operation,
                    EntityName = entityName
                };

                context.Add(auditLog);
            }
        }

        private object? GetKeyValue(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            if (key == null) return null;

            var keyValues = new Dictionary<string, object>();
            foreach (var property in key.Properties)
            {
                var value = entry.Property(property.Name).CurrentValue;
                if (value != null)
                {
                    keyValues[property.Name] = value;
                }
            }

            return keyValues.Count > 0 ? keyValues : null;
        }

        private string GetRecordLabel(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            // Try to generate a human-readable label for the record
            // This is a basic implementation - can be enhanced based on entity types
            try
            {
                var entity = entry.Entity;
                if (entity == null) return "Unknown";

                // For common entities, try to find a name/label property
                var nameProperty = entity.GetType().GetProperty("Name") ?? 
                                  entity.GetType().GetProperty("FirstName") ??
                                  entity.GetType().GetProperty("Title") ??
                                  entity.GetType().GetProperty("Description");

                if (nameProperty != null)
                {
                    var value = nameProperty.GetValue(entity);
                    if (value != null)
                    {
                        return value.ToString() ?? "Unknown";
                    }
                }

                return entity.GetType().Name;
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
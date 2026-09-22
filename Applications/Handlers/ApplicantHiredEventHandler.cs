using MediatR;
using Applications.Events;

namespace Applications.Handlers
{
    public class ApplicantHiredEventHandler : INotificationHandler<ApplicantHiredEvent>
    {
        private readonly ILogger<ApplicantHiredEventHandler> _logger;

        public ApplicantHiredEventHandler(ILogger<ApplicantHiredEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(ApplicantHiredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("EVENT RECEIVED: Applicant {ApplicantId} hired as Employee {EmployeeId}. Provisioning ABAC credentials...", 
                notification.ApplicantId, notification.EmployeeId);
            
            // Future integration points:
            // - Send welcome email to employee
            // - Notify payroll system for onboarding
            // - Update ERP system records
            
            await Task.CompletedTask;
        }
    }
}
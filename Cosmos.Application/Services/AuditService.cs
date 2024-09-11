using Cosmos.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Cosmos.Application.Services
{
    public class AuditService : IAuditService
    {

        private readonly IAuditRepository _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IAuditRepository auditRepository, IHttpContextAccessor httpContextAccessor)
        {
            _auditRepository = auditRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // Modified to accept additional parameters
        public async Task LogAuditAsync(string action, int responseStatusCode, string userId, string userName, string ipAddress)
        {
            var audit = new Audit
            {
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                Action = action,
                ResponseStatusCode = responseStatusCode.ToString(),
                Timestamp = DateTime.UtcNow
            };

            await _auditRepository.AddAuditAsync(audit);
        }

        public async Task<IEnumerable<Audit>> GetAllAuditLogsAsync()
        {
            var query = "SELECT * FROM c";
            return await _auditRepository.GetAuditLogsAsync(query);
        }

        // Get audit logs by user ID
        public async Task<IEnumerable<Audit>> GetAuditLogsByUserAsync(string userId)
        {
            var query = $"SELECT * FROM c WHERE c.UserId = '{userId}'";
            return await _auditRepository.GetAuditLogsAsync(query);
        }

        // Get audit logs by action
        public async Task<IEnumerable<Audit>> GetAuditLogsByActionAsync(string action)
        {
            var query = $"SELECT * FROM c WHERE CONTAINS(LOWER(c.Action), '{action.ToLower()}')";
            return await _auditRepository.GetAuditLogsAsync(query);
        }

        // Get audit logs by date range
        public async Task<IEnumerable<Audit>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = $"SELECT * FROM c WHERE c.Timestamp >= '{startDate:O}' AND c.Timestamp <= '{endDate:O}'";
            return await _auditRepository.GetAuditLogsAsync(query);
        }

    }
}

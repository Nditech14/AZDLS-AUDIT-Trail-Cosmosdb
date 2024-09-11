using Cosmos.Core.Entities;

namespace Cosmos.Application.Services
{
    public interface IAuditService
    {
        Task<IEnumerable<Audit>> GetAllAuditLogsAsync();
        Task<IEnumerable<Audit>> GetAuditLogsByActionAsync(string action);
        Task<IEnumerable<Audit>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Audit>> GetAuditLogsByUserAsync(string userId);
        Task LogAuditAsync(string action, int responseStatusCode, string userId, string userName, string ipAddress);
    }
}
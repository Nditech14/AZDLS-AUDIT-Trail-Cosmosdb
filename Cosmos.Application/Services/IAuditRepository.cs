using Cosmos.Core.Entities;

namespace Cosmos.Application.Services
{
    public interface IAuditRepository
    {
        Task AddAuditAsync(Audit audit);
        Task<IEnumerable<Audit>> GetAuditLogsAsync(string query);
    }
}
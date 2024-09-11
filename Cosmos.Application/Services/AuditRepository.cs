using Cosmos.Core.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Cosmos.Application.Services
{
    public class AuditRepository : IAuditRepository
    {
        private readonly Container _auditContainer;

        public AuditRepository(CosmosClient cosmosClient, IConfiguration configuration)
        {
            var databaseName = configuration["CosmosDb:DatabaseName"];
            _auditContainer = cosmosClient.GetContainer(databaseName, configuration["CosmosDb:Containers:AuditContainer"]);
        }

        public async Task AddAuditAsync(Audit audit)
        {
            audit.id = Guid.NewGuid().ToString();
            await _auditContainer.CreateItemAsync(audit);
        }

        public async Task<IEnumerable<Audit>> GetAuditLogsAsync(string query)
        {
            var iterator = _auditContainer.GetItemQueryIterator<Audit>(new QueryDefinition(query));
            var results = new List<Audit>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
    }
}

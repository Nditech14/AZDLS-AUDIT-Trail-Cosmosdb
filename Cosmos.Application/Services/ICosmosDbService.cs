using Microsoft.Azure.Cosmos;

namespace Cosmos.Application.Services
{
    public interface ICosmosDbService<T>
    {
        Task AddItemAsync(T item);
        Task DeleteItemAsync(string id, PartitionKey partitionKey);
        Task<T> GetItemAsync(string id, PartitionKey partitionKey);
        FeedIterator<T> GetItemQueryIterator(QueryDefinition query, string continuationToken = null, QueryRequestOptions requestOptions = null);
       // Task GetItemsAsync(QueryDefinition queryDefinition);
        Task<IEnumerable<T>> GetItemsAsync(string query);
        Task<(IEnumerable<T> Items, string ContinuationToken)> GetItemsWithContinuationTokenAsync(string continuationToken, int maxItemCount);
        Task UpdateItemAsync(string id, T item);
    }
}
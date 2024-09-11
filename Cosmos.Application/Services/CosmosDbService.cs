using Cosmos.Application.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Application.Services
{
    public class CosmosDbService<T> : ICosmosDbService<T>
    {
        private readonly Container _productsContainer;
        private readonly Container _categoriesContainer;
        private readonly Container _subCategoryContainer;

        public CosmosDbService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            var databaseName = configuration["CosmosDb:DatabaseName"];
           _productsContainer = cosmosClient.GetContainer(databaseName, configuration["CosmosDb:Containers:ProductsContainer"]);
           _categoriesContainer = cosmosClient.GetContainer(databaseName, configuration["CosmosDb:Containers:CategoriesContainer"]);
            _subCategoryContainer = cosmosClient.GetContainer(databaseName, configuration["CosmosDb:Containers:SubCategoryContainerName"]);
        }


        // Method to determine which container to use based on the entity type
        private Container GetContainer()
        {
            if (typeof(T) == typeof(Product))
            {
                return _productsContainer;
            }
            else if (typeof(T) == typeof(Category))
            {
                return _categoriesContainer;
            }
            else if (typeof(T) == typeof(SubCategory))
            {
                return _subCategoryContainer;
            }
            else
            {
                throw new ArgumentException($"No container available for type {typeof(T).Name}");
            }
        }

        public async Task AddItemAsync(T item)
        {
            var container = GetContainer();
            await container.CreateItemAsync(item);
        }

        public async Task DeleteItemAsync(string id, PartitionKey partitionKey)
        {
            var container = GetContainer();
            await container.DeleteItemAsync<T>(id, partitionKey);
        }

        public async Task<T> GetItemAsync(string id, PartitionKey partitionKey)
        {
            var container = GetContainer();
            try
            {
                var response = await container.ReadItemAsync<T>(id, partitionKey);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(string query)
        {
            var container = GetContainer();
            var iterator = container.GetItemQueryIterator<T>(new QueryDefinition(query));
            var results = new List<T>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, T item)
        {
            var container = GetContainer();
            await container.UpsertItemAsync(item, new PartitionKey(id));
        }

        //public async Task<(IEnumerable<T> Items, string ContinuationToken)> GetItemsWithContinuationTokenAsync(string continuationToken, int maxItemCount)
        //{
        //    var container = GetContainer();  // Get the appropriate container
        //    var queryRequestOptions = new QueryRequestOptions { MaxItemCount = maxItemCount };
        //    var queryIterator = container.GetItemQueryIterator<T>(continuationToken: continuationToken, requestOptions: queryRequestOptions);

        //    var results = new List<T>();
        //    string newContinuationToken = null;

        //    while (queryIterator.HasMoreResults)
        //    {
        //        var response = await queryIterator.ReadNextAsync();
        //        results.AddRange(response);
        //        newContinuationToken = response.ContinuationToken;

        //        if (results.Count >= maxItemCount)
        //        {
        //            break;
        //        }
        //    }

        //    return (results, newContinuationToken);
        //}

        public async Task<(IEnumerable<T> Items, string ContinuationToken)> GetItemsWithContinuationTokenAsync(string continuationToken, int maxItemCount = 30)
        {
            var container = GetContainer();
            var queryRequestOptions = new QueryRequestOptions { MaxItemCount = maxItemCount };
            var queryIterator = container.GetItemQueryIterator<T>(continuationToken: continuationToken, requestOptions: queryRequestOptions);

            var results = new List<T>();
            string newContinuationToken = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                results.AddRange(response);
                newContinuationToken = response.ContinuationToken;

                if (results.Count >= maxItemCount)
                {
                    break;
                }
            }

            return (results, newContinuationToken);
        }



        public FeedIterator<T> GetItemQueryIterator(QueryDefinition query, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            var container = GetContainer();
            return container.GetItemQueryIterator<T>(query, continuationToken, requestOptions);
        }

        //public Task GetItemsAsync(QueryDefinition queryDefinition)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

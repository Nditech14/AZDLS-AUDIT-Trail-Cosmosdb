using Cosmos.Application.Entities;
using Cosmos.Core.DTO;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Application.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly ICosmosDbService<SubCategory> _cosmosDbService;


        public SubCategoryService(ICosmosDbService<SubCategory> cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task CreateSubCategoryAsync(SubCategory subCategory)
        {
            await _cosmosDbService.AddItemAsync(subCategory);
        }

        public async Task UpdateSubCategoryAsync(string id, UpdateSubCategoryDto updateDto)
        {
            var subCategory = await _cosmosDbService.GetItemAsync(id, new PartitionKey(id));
            if (subCategory != null)
            {
                subCategory.SubCategoryName = updateDto.SubCategoryName;
                await _cosmosDbService.UpdateItemAsync(id, subCategory);
            }
        }

        public async Task<SubCategory> GetSubCategoryByIdAsync(string id)
        {
            return await _cosmosDbService.GetItemAsync(id, new PartitionKey(id));
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategoriesAsync()
        {
            var query = "SELECT * FROM c";
            return await _cosmosDbService.GetItemsAsync(query);
        }

        public async Task DeleteSubCategoryAsync(string id)
        {
            await _cosmosDbService.DeleteItemAsync(id, new PartitionKey(id));
        }

        public async Task<(IEnumerable<SubCategory> subCategories, string ContinuationToken)> GetSubCategoriesAsync(string continuationToken = null)
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var queryRequestOptions = new QueryRequestOptions { MaxItemCount = 5 };
            var subCategoryIterator = _cosmosDbService.GetItemQueryIterator(query, continuationToken, queryRequestOptions);
            var subCategories = new List<SubCategory>();
            string newContinuationtoken = null;
            while (subCategoryIterator.HasMoreResults)
            {
                var response = await subCategoryIterator.ReadNextAsync();
                if (response == null || response.Count == 0)
                {
                    break;
                }
                subCategories.AddRange(response);
                newContinuationtoken = response.ContinuationToken;
                if (subCategories.Count > 5)
                {
                    break;
                }
            }
            return (subCategories, newContinuationtoken);


        }
    }
}



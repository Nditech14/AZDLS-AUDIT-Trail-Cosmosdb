using Cosmos.Application.Entities;
using Cosmos.Core.DTO;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICosmosDbService<Category> _cosmosDbService;
        private readonly ICosmosDbService<SubCategory> _subCategoriesDbService;

        public CategoryService(ICosmosDbService<Category> cosmosDbService, ICosmosDbService<SubCategory> subCategoriesDbService)
        {
            _cosmosDbService = cosmosDbService;
            _subCategoriesDbService = subCategoriesDbService;


        }


        public async Task<CategoryDto> AddCategoryAsync(Category category)
        {
            // Generate a new ID for the category
            category.id = Guid.NewGuid().ToString();

            // Adding subcategories to the database
            foreach (var subcategory in category.subCategories)
            {
                subcategory.id = Guid.NewGuid().ToString();
                subcategory.CategoryId = category.id;
                await _subCategoriesDbService.AddItemAsync(subcategory);
            }

            // Adding the category to the database
            await _cosmosDbService.AddItemAsync(category);

            // Returning the CategoryDto after successful addition
            var categoryDto = new CategoryDto
            {

                CategoryName = category.CategoryName,
                SubCategories = category.subCategories.Select(sc => new SubCategoryDto
                {

                    SubCategoryName = sc.SubCategoryName,

                }).ToList()
            };

            return categoryDto;
        }

        public async Task<Category> UpdateCategoryAsync(string id, Category categoryDto)
        {
            var category = await _cosmosDbService.GetItemAsync(id, new PartitionKey(id));

            if (category != null)
            {
               
                category.CategoryName = categoryDto.CategoryName;

                
                await _cosmosDbService.UpdateItemAsync(id, category);
            }

            return categoryDto;
        }
        
        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            var query = $"SELECT * FROM c WHERE c.Name = '{categoryName}'";
            var categories = await _cosmosDbService.GetItemsAsync(query);
            var category = categories.FirstOrDefault();

            return category != null ? new Category { id = category.id, CategoryName = category.CategoryName } : null;
        }

        
        public async Task<Category> GetCategoryByIdAsync(string id)
        {
            var category = await _cosmosDbService.GetItemAsync(id, new PartitionKey(id));
            return category != null ? new Category { id = category.id, CategoryName = category.CategoryName } : null;
        }

        
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var query = "SELECT * FROM c";
            var categories = await _cosmosDbService.GetItemsAsync(query);

            return categories.Select(c => new Category { id = c.id, CategoryName = c.CategoryName });
        }
        public async Task<(IEnumerable<Category> Categories, string ContinuationToken)> GetCategoriesAsync(string continuationToken = null)
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var queryRequestOptions = new QueryRequestOptions { MaxItemCount = 5 };
            var categoryIterator = _cosmosDbService.GetItemQueryIterator(query, continuationToken, queryRequestOptions);
            var categories = new List<Category>();
            string newContinuationToken = null;

            while (categoryIterator.HasMoreResults)
            {
                var response = await categoryIterator.ReadNextAsync();
                if (response == null || response.Count == 0)
                {
                    break;
                }

                categories.AddRange(response);
                newContinuationToken = response.ContinuationToken;

                if (categories.Count >= 5)
                {
                    break;
                }
            }

            return (categories.Take(5), newContinuationToken);
        }

    }
}

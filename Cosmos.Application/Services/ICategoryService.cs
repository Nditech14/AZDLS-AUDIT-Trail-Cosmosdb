using Cosmos.Application.Entities;
using Cosmos.Core.DTO;

namespace Cosmos.Application.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> AddCategoryAsync(Category category);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<(IEnumerable<Category> Categories, string ContinuationToken)> GetCategoriesAsync(string continuationToken = null);
        Task<Category> GetCategoryByIdAsync(string id);
        Task<Category> GetCategoryByNameAsync(string categoryName);
        
        Task<Category> UpdateCategoryAsync(string id, Category categoryDto);
    }
}
using Cosmos.Application.Entities;
using Cosmos.Core.DTO;

namespace Cosmos.Application.Services
{
    public interface ISubCategoryService
    {
        Task CreateSubCategoryAsync(SubCategory subCategory);
        Task DeleteSubCategoryAsync(string id);
        Task<IEnumerable<SubCategory>> GetAllSubCategoriesAsync();
        Task<(IEnumerable<SubCategory> subCategories, string ContinuationToken)> GetSubCategoriesAsync(string continuationToken = null);
        Task<SubCategory> GetSubCategoryByIdAsync(string id);
        Task UpdateSubCategoryAsync(string id, UpdateSubCategoryDto updateDto);
    }
}
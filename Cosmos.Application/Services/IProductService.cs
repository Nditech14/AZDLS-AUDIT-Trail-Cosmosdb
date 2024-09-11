using Cosmos.Application.Entities;
using Cosmos.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace Cosmos.Application.Services
{
    public interface IProductService
    {
        Task AddProductAsync(Product product);
        Task BatchDeleteProductsAsync(List<string> productIds);
        Task BatchUpdateProductsAsync(List<string> productIds, List<UpdateProductDto> productsToUpdate);
        Task DeleteProductAsync(string id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(string id);
        Task<ResponseDto<(IEnumerable<ProductResponseDto> Products, string ContinuationToken)>> GetProductsAsync(string continuationToken, int pageSize);
        Task OutOfStockProductAsync(string id);
        Task<IEnumerable<Product>> SearchProductsAsync(string productName = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task SoftDeleteProductAsync(string id);
        Task UpdateProductAsync(string id, UpdateProductDto updateDto);
        Task UpdateProductImagesAsync(string id, List<IFormFile> images);
    }
}
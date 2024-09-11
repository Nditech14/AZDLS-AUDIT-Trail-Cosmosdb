using AutoMapper;
using Cosmos.Application.Entities;
using Cosmos.Core.DTO;
using Cosmos.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Security.Claims;

namespace Cosmos.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ICosmosDbService<Product> _productService;
        private readonly ICosmosDbService<Category> _categoryService;
        private readonly ICosmosDbService<SubCategory> _subCategoryService;
        private readonly IFileService<FileEntity> _fileService;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ProductService(
            ICosmosDbService<Product> productService,
            ICosmosDbService<Category> categoryService,
            ICosmosDbService<SubCategory> subCategoryService,
            IFileService<FileEntity> fileService,
            IAuditService auditService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _productService = productService;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _fileService = fileService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private (string userId, string userRole, string userName) GetUserClaims()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ipAddress = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.GivenName)?.Value; // Corrected to GivenName
            return (userId, ipAddress, userName);
        }

        public async Task AddProductAsync(Product product)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                product.id = Guid.NewGuid().ToString(); // Assign new unique ID
                product.Status = Status.Active;
                // Assign unique IDs for variants and specifications
                foreach (var variant in product.Variants)
                {
                    variant.ProductId = product.id;
                    variant.id = Guid.NewGuid().ToString();
                }

                foreach (var specification in product.Specifications)
                {
                    specification.ProductId = product.id;
                    specification.id = Guid.NewGuid().ToString();
                }

                // Handle categories and subcategories
                foreach (var category in product.Categories)
                {
                    category.id = Guid.NewGuid().ToString();
                    category.ProductId = product.id;

                    foreach (var subCategory in category.subCategories)
                    {
                        subCategory.id = Guid.NewGuid().ToString();
                        subCategory.CategoryId = category.id;
                        await _subCategoryService.AddItemAsync(subCategory);
                    }
                    await _categoryService.AddItemAsync(category);
                }

                await _productService.AddItemAsync(product);
                await _auditService.LogAuditAsync($"Added product with ID: {product.id}", (int)HttpStatusCode.Created, userId, userName, ipAddress);
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync($"Failed to add product", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                throw new Exception("Error adding product.", ex);
            }
        }

        public async Task UpdateProductAsync(string id, UpdateProductDto updateDto)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                var product = await _productService.GetItemAsync(id, new PartitionKey(id));

                if (product == null)
                {
                    await _auditService.LogAuditAsync($"Product with ID: {id} not found", (int)HttpStatusCode.NotFound, userId, userName, ipAddress);
                    throw new KeyNotFoundException($"Product with ID: {id} not found.");
                }

                // Apply updates if the values are not null or empty
                if (!string.IsNullOrWhiteSpace(updateDto.ProductName))
                    product.Title = updateDto.ProductName;

                if (!string.IsNullOrWhiteSpace(updateDto.Description))
                    product.Description = updateDto.Description;
                if (updateDto.Status.HasValue)
                    product.Status = updateDto.Status.Value;

                if (updateDto.Price.HasValue)
                    product.Price = updateDto.Price.Value;

                product.UpdatedOnUtc = DateTime.UtcNow;
                await _productService.UpdateItemAsync(id, product);

                await _auditService.LogAuditAsync($"Updated product with ID: {id}", (int)HttpStatusCode.OK, userId, userName, ipAddress);
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync($"Failed to update product with ID: {id}", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                throw new Exception($"Error updating product with ID: {id}.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                var products = await _productService.GetItemsAsync("SELECT * FROM c");
                await _auditService.LogAuditAsync("Retrieved all products", (int)HttpStatusCode.OK, userId, userName, ipAddress);
                return products;
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync("Failed to retrieve products", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                throw new Exception("Error retrieving products.", ex);
            }
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                var product = await _productService.GetItemAsync(id, new PartitionKey(id));
                if (product == null)
                {
                    await _auditService.LogAuditAsync($"Product with ID: {id} not found", (int)HttpStatusCode.NotFound, userId, userName, ipAddress);
                    throw new KeyNotFoundException($"Product with ID: {id} not found.");
                }

                await _auditService.LogAuditAsync($"Retrieved product with ID: {id}", (int)HttpStatusCode.OK, userId, userName, ipAddress);
                return product;
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync($"Failed to retrieve product with ID: {id}", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                throw new Exception($"Error retrieving product with ID: {id}.", ex);
            }
        }

        public async Task DeleteProductAsync(string id)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                var product = await _productService.GetItemAsync(id, new PartitionKey(id));
                if (product == null)
                {
                    await _auditService.LogAuditAsync($"Product with ID: {id} not found", (int)HttpStatusCode.NotFound, userId, userName, ipAddress);
                    throw new KeyNotFoundException($"Product with ID: {id} not found.");
                }

                product.Status = Status.Deleted;
                await _productService.DeleteItemAsync(id, new PartitionKey(id));
                await _auditService.LogAuditAsync($"Deleted product with ID: {id}", (int)HttpStatusCode.OK, userId, userName, ipAddress);
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync($"Failed to delete product with ID: {id}", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                throw new Exception($"Error deleting product with ID: {id}.", ex);
            }
        }

        public async Task<ResponseDto<(IEnumerable<ProductResponseDto> Products, string ContinuationToken)>> GetProductsAsync(string continuationToken, int pageSize)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                var result = await _productService.GetItemsWithContinuationTokenAsync(continuationToken, pageSize);
                var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(result.Items);

                await _auditService.LogAuditAsync($"Retrieved products with continuation token: {continuationToken}", (int)HttpStatusCode.OK, userId, userName, ipAddress);
                return ResponseDto<(IEnumerable<ProductResponseDto> Products, string ContinuationToken)>
                    .Success((productDtos, result.ContinuationToken), "Products retrieved successfully.");
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync("Failed to retrieve products", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                var errors = new List<Error> { new Error("GetProductsError", ex.Message) };
                return ResponseDto<(IEnumerable<ProductResponseDto> Products, string ContinuationToken)>
                    .Failure(errors, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task SoftDeleteProductAsync(string id)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                var product = await _productService.GetItemAsync(id, new PartitionKey(id));
                if (product == null)
                {
                    await _auditService.LogAuditAsync($"Product with ID: {id} not found", (int)HttpStatusCode.NotFound, userId, userName, ipAddress);
                    throw new KeyNotFoundException($"Product with ID: {id} not found.");
                }

                // Mark the product as deleted
                product.Status = Status.Deleted;
                product.UpdatedOnUtc = DateTime.UtcNow;
                await _productService.UpdateItemAsync(id, product);
                await _auditService.LogAuditAsync($"Soft deleted product with ID: {id}", (int)HttpStatusCode.OK, userId, userName, ipAddress);
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync($"Failed to soft delete product with ID: {id}", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                throw new Exception($"Error soft deleting product with ID: {id}.", ex);
            }
        }

        public async Task OutOfStockProductAsync(string id)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                var product = await _productService.GetItemAsync(id, new PartitionKey(id));
                if (product == null)
                {
                    await _auditService.LogAuditAsync($"Product with ID: {id} not found", (int)HttpStatusCode.NotFound, userId, userName, ipAddress);
                    throw new KeyNotFoundException($"Product with ID: {id} not found.");
                }

                // Mark the product as out of stock
                product.Status = Status.OutofStock;
                product.UpdatedOnUtc = DateTime.UtcNow;
                await _productService.UpdateItemAsync(id, product);
                await _auditService.LogAuditAsync($"Product with ID: {id} marked as out of stock", (int)HttpStatusCode.OK, userId, userName, ipAddress);
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync($"Failed to mark product with ID: {id} as out of stock", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                throw new Exception($"Error marking product with ID: {id} as out of stock.", ex);
            }
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string productName = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = "SELECT * FROM c WHERE c.Status != 'Deleted'";
            if (!string.IsNullOrEmpty(productName))
                query += $" AND CONTAINS(c.Title, '{productName}')";
            if (minPrice.HasValue)
                query += $" AND c.Price >= {minPrice.Value}";
            if (maxPrice.HasValue)
                query += $" AND c.Price <= {maxPrice.Value}";


            return await _productService.GetItemsAsync(query);
        }

        public async Task BatchDeleteProductsAsync(List<string> productIds)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            foreach (var id in productIds)
            {
                try
                {
                    var product = await _productService.GetItemAsync(id, new PartitionKey(id));
                    if (product == null)
                    {
                        await _auditService.LogAuditAsync($"Product with ID: {id} not found", (int)HttpStatusCode.NotFound, userId, userName, ipAddress);
                        continue;
                    }

                    product.Status = Status.Deleted;
                    await _productService.UpdateItemAsync(id, product);
                }
                catch (Exception ex)
                {
                    await _auditService.LogAuditAsync($"Failed to delete product with ID: {id}", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                }
            }
            await _auditService.LogAuditAsync("Batch delete operation completed", (int)HttpStatusCode.OK, userId, userName, ipAddress);
        }

        public async Task BatchUpdateProductsAsync(List<string> productIds, List<UpdateProductDto> productsToUpdate)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            for (int i = 0; i < productIds.Count; i++)
            {
                string productId = productIds[i];
                UpdateProductDto productDto = productsToUpdate[i];

                try
                {
                    var product = await _productService.GetItemAsync(productId, new PartitionKey(productId));
                    if (product == null)
                    {
                        await _auditService.LogAuditAsync($"Product with ID: {productId} not found", (int)HttpStatusCode.NotFound, userId, userName, ipAddress);
                        continue;
                    }

                    // Update product details
                    product.Title = productDto.ProductName ?? product.Title;
                    product.Description = productDto.Description ?? product.Description;
                    product.Price = productDto.Price ?? product.Price;
                    product.Status = productDto.Status ?? product.Status;
                    product.UpdatedOnUtc = DateTime.UtcNow;

                    await _productService.UpdateItemAsync(productId, product);
                }
                catch (Exception ex)
                {
                    await _auditService.LogAuditAsync($"Failed to update product with ID: {productId}", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                }
            }
            await _auditService.LogAuditAsync("Batch update operation completed", (int)HttpStatusCode.OK, userId, userName, ipAddress);
        }

        public async Task UpdateProductImagesAsync(string id, List<IFormFile> images)
        {
            var (userId, ipAddress, userName) = GetUserClaims();
            try
            {
                var product = await _productService.GetItemAsync(id, new PartitionKey(id));
                if (product == null)
                {
                    await _auditService.LogAuditAsync($"Product with ID: {id} not found", (int)HttpStatusCode.NotFound, userId, userName, ipAddress);
                    throw new KeyNotFoundException($"Product with ID: {id} not found.");
                }

                if (product.MediaPaths == null)
                {
                    product.MediaPaths = new List<FileEntity>();
                }

                foreach (var image in images)
                {
                    using var fileStream = image.OpenReadStream();
                    var uploadedFile = await _fileService.UploadFileAsync(fileStream, image.FileName);
                    product.MediaPaths.Add(uploadedFile);
                }

                await _productService.UpdateItemAsync(id, product);
                await _auditService.LogAuditAsync($"Updated images for product with ID: {id}", (int)HttpStatusCode.OK, userId, userName, ipAddress);
            }
            catch (Exception ex)
            {
                await _auditService.LogAuditAsync($"Failed to update images for product with ID: {id}", (int)HttpStatusCode.InternalServerError, userId, userName, ipAddress);
                throw new Exception($"Error updating images for product with ID: {id}.", ex);
            }
        }
    }
}

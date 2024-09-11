using AutoMapper;
using Cosmos.Application.Entities;
using Cosmos.Application.Services;
using Cosmos.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cosmos.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        // ========== GET Operations ==========

        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound($"Product with id {id} not found.");

            return Ok(new { message = product });
        }

        [HttpGet("all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        [HttpGet("load-all-products")]
        public async Task<IActionResult> GetProducts([FromQuery] string continuationToken = null)
        {
            var pageSize = 30;
            var response = await _productService.GetProductsAsync(continuationToken, pageSize);

            if (response.IsSuccessful)
            {
                return Ok(new
                {
                    Products = response.Data.Products,
                    ContinuationToken = response.Data.ContinuationToken
                });
            }

            return StatusCode(response.Code, response);
        }

        [HttpGet("SearchProducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string productName = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
        {
            var products = await _productService.SearchProductsAsync(productName, minPrice, maxPrice);
            return Ok(products);
        }

        // ========== POST Operations ==========

        //[HttpPost("AddProduct")]
        //public async Task<ActionResult<ProductDto>> AddProduct([FromBody] ProductCreationDto product)
        //{
        //    var domainProduct = _mapper.Map<Product>(product);
        //    if (domainProduct == null)
        //        return BadRequest("Product data is required.");

        //    await _productService.AddProductAsync(domainProduct);
        //    var addedProduct = await _productService.GetProductByIdAsync(domainProduct.id);

        //    return CreatedAtAction(nameof(GetProductById), new { id = domainProduct.id }, new { message = addedProduct });
        //}

        [HttpPost("AddProducts")]
        public async Task<ActionResult> AddProducts([FromBody] List<ProductCreationDto> products)
        {
            if (products == null || !products.Any())
                return BadRequest("At least one product data is required.");

            var domainProducts = _mapper.Map<List<Product>>(products);

            if (domainProducts == null || !domainProducts.Any())
                return BadRequest("Product data is invalid.");

            foreach (var product in domainProducts)
            {
                await _productService.AddProductAsync(product);
            }

            var addedProductIds = domainProducts.Select(p => p.id).ToList();

            return CreatedAtAction(nameof(GetProducts), new { ids = string.Join(",", addedProductIds) }, new { message = "Products added successfully." });
        }

        [HttpPost("BatchDeleteProducts")]
        public async Task<ActionResult> BatchDeleteProducts([FromBody] List<string> productIds)
        {
            if (productIds == null || !productIds.Any())
                return BadRequest("Product IDs are required.");

            await _productService.BatchDeleteProductsAsync(productIds);
            return Ok(new { Message = "Batch delete operation completed." });
        }

        [HttpPost("BatchUpdateProducts")]
        public async Task<ActionResult> BatchUpdateProducts([FromBody] List<UpdateProductDto> productsToUpdate, [FromQuery] List<string> productIds)
        {
            if (productsToUpdate == null || !productsToUpdate.Any() || productIds == null || productIds.Count != productsToUpdate.Count)
                return BadRequest("Product update data and corresponding IDs are required.");

            await _productService.BatchUpdateProductsAsync(productIds, productsToUpdate);
            return Ok(new { Message = "Batch update operation completed." });
        }

        // ========== PUT Operations ==========

        [HttpPut("AddImagesToProduct/{id}")]
        public async Task<ActionResult> AddImagesToProduct(string id, [FromForm] List<IFormFile> images)
        {
            await _productService.UpdateProductImagesAsync(id, images);
            return Ok(new { Message = "Product images added successfully" });
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<ActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
                return BadRequest("Product data is required.");

            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound($"Product with id {id} not found.");

            await _productService.UpdateProductAsync(id, updateProductDto);

            var latestUpdatedProduct = await _productService.GetProductByIdAsync(id);

            return Ok(new
            {
                Product = latestUpdatedProduct,
                Message = "Product updated successfully"
            });
        }

        // ========== PATCH Operations ==========

        [HttpPatch("out-of-stock/{id}")]
        public async Task<IActionResult> MarkProductOutOfStock(string id)
        {
            try
            {
                await _productService.OutOfStockProductAsync(id);
                return Ok($"Product with ID: {id} marked as out of stock");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // ========== DELETE Operations ==========

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound($"Product with id {id} not found.");

            await _productService.DeleteProductAsync(id);
            return Ok(new { message = product });
        }

        [HttpDelete("SoftDeleteProduct/{id}")]
        public async Task<ActionResult> SoftDeleteProduct(string id)
        {
            await _productService.SoftDeleteProductAsync(id);
            return Ok(new { Message = $"Product with ID {id} has been soft deleted." });
        }
    }
}

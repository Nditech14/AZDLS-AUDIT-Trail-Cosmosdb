using AutoMapper;
using Cosmos.Application.Entities;
using Cosmos.Application.Services;
using Cosmos.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMapper _mapper;

        public SubCategoryController(ISubCategoryService subCategoryService, IMapper mapper)
        {
            _subCategoryService = subCategoryService;
            _mapper = mapper;
        }

        
        [HttpGet("GetSubCategoryById/{id}")]
        public async Task<ActionResult> GetSubCategoryById(string id)
        {
            var subCategory = await _subCategoryService.GetSubCategoryByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound(new { Message = "Subcategory not found" });
            }
            var existingSubCategory = _mapper.Map<SubCategoryDto>(subCategory);

            var response = new
            {
                Message = "Subcategory retrieved successfully",
                Data = existingSubCategory
            };
            return Ok(response);
        }

      
        [HttpPut("UpdateSubCategory/{id}")]
        public async Task<ActionResult> UpdateSubCategory(string id, [FromBody] UpdateSubCategoryDto updateDto)
        {
            var subCategory = await _subCategoryService.GetSubCategoryByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound(new { Message = "Subcategory not found" });
            }

            await _subCategoryService.UpdateSubCategoryAsync(id, updateDto);
            var response = new
            {
                Message = "Subcategory updated successfully",
                Data = subCategory
            };

            return Ok(response);
        }

       
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteSubCategory(string id)
        {
            var subCategory = await _subCategoryService.GetSubCategoryByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound(new { Message = "Subcategory not found" });
            }

            await _subCategoryService.DeleteSubCategoryAsync(id);
            var response = new
            {
                Message = "Subcategory deleted successfully"
            };

            return Ok(response);
        }

        [HttpGet("LoadMore")]
        public async Task<IActionResult> LoadMore([FromQuery] string continuationToken = null)
        {
            var (subCategories, newContinuationToken) = await _subCategoryService.GetSubCategoriesAsync(continuationToken);

            if (subCategories == null || !subCategories.Any())
            {
                return NotFound("No subcategories found.");
            }

            return Ok(new
            {
                SubCategories = subCategories,
                ContinuationToken = newContinuationToken
            });
        }
    }
}

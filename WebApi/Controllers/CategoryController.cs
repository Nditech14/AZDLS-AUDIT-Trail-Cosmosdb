using AutoMapper;
using Cosmos.Application.Entities;
using Cosmos.Application.Services;
using Cosmos.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (updateCategoryDto == null)
                return BadRequest("Category data is required.");

            
            var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
            if (existingCategory == null)
                return NotFound($"Category with id {id} not found.");

            
            await _categoryService.UpdateCategoryAsync(id, existingCategory);

           
            var updatedCategory = await _categoryService.GetCategoryByIdAsync(id);

         
            return Ok(new
            {
                Category = updatedCategory,
                Message = "Category updated successfully"
            });
        }




        [HttpGet("searchById/{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
                return NotFound(new { message = $"No Category with {id} " });

            return Ok(new { message = category });
        }


        [HttpGet]
        [Route("categories-LoadMore")]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] string continuationToken = null)
        {
            var (categories, newContinuationToken) = await _categoryService.GetCategoriesAsync(continuationToken);

            var response = new
            {
                Categories = categories,
                ContinuationToken = newContinuationToken
            };

            return Ok(response);



        }
    }
}
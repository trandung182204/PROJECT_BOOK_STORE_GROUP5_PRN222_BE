using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await categoryService.GetAllCategoryAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        // ✅ POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (category == null)
                return BadRequest("Invalid request body.");

            if (string.IsNullOrWhiteSpace(category.CategoryCode))
                return BadRequest("CategoryCode is required.");
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                return BadRequest("CategoryName is required.");
            if (await categoryService.ExistsAsync(category.CategoryCode, category.CategoryName))
                return BadRequest("CategoryCode or CategoryName already exists.");

            category.IsDeleted = false;
            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = null;

            await categoryService.AddCategoryAsync(category);
            return Ok(new { message = "Category created successfully.", category });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Category category)
        {
            if (id != category.Id)
                return BadRequest("ID mismatch.");
            if (await categoryService.ExistsAsync(category.CategoryCode, category.CategoryName))
                return BadRequest("CategoryCode or CategoryName already exists.");

            await categoryService.UpdateCategoryAsync(category);
            return Ok(new { message = "Category updated successfully.", category });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search keyword cannot be empty.");

            var categories = await categoryService.GetAllCategoryAsync();
            var result = categories.Where(c => c.CategoryName.Contains(name, StringComparison.OrdinalIgnoreCase) && !c.IsDeleted);

            return Ok(result);
        }
    }
}

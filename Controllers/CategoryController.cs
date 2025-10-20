using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoryService.GetAllCategoryAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDTO category)
        {
            var response = await _categoryService.AddCategoryAsync(category);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] CategoryDTO category)
        {
            var response = await _categoryService.UpdateCategoryAsync(id, category);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var all = await _categoryService.GetAllCategoryAsync();
            if (!all.Succeeded)
                return Ok(all);

            var list = all.Data as IEnumerable<Category>;
            var result = list!.Where(c => c.CategoryName.Contains(name, StringComparison.OrdinalIgnoreCase) && !c.IsDeleted).ToList();

            return Ok(new ApiRespone
            {
                Succeeded = true,
                Message = $"Found {result.Count} categories matching '{name}'.",
                Data = result
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleBooksController : ControllerBase
    {
        private readonly GoogleBooksService _googleBooksService;

        public GoogleBooksController(GoogleBooksService googleBooksService)
        {
            _googleBooksService = googleBooksService;
        }

        // ✅ Import theo keyword (ví dụ: "harry potter")
        [HttpPost("import-by-keyword")]
        public async Task<IActionResult> ImportByKeyword([FromQuery] string keyword)
        {
            int imported = await _googleBooksService.ImportBooksFromGoogleAsync(keyword, 10);
            return Ok(new { message = $"Imported {imported} books by keyword: {keyword}" });
        }

        // ✅ Import theo category (ví dụ: "fiction", "history", "science")
        [HttpPost("import-by-category")]
        public async Task<IActionResult> ImportByCategory([FromQuery] string category)
        {
            int imported = await _googleBooksService.ImportBooksByCategoryAsync(category, 10);
            return Ok(new { message = $"Imported {imported} books for category: {category}" });
        }
    }
}

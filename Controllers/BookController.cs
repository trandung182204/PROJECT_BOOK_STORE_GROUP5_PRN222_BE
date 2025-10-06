using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService bookService;

        public BookController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await bookService.GetAllBookAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await bookService.GetBookByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            await bookService.AddBookAsync(book);
            return Ok(book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            await bookService.UpdateBookAsync(book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await bookService.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string title)
        {
            var books = await bookService.GetAllBookAsync();
            var result = books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            return Ok(result);
        }
    }
}

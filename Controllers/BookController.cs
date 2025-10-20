using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Helpers;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // Lấy tất cả sách
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _bookService.GetAllBookAsync();
            return Ok(response);
        }

        // Lấy chi tiết theo ID
        [HttpGet("{id}")]
   
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _bookService.GetBookByIdAsync(id);
            return Ok(response);
        }

        // Thêm sách mới
        [HttpPost]

        public async Task<IActionResult> Create([FromBody] BookDTO book)
        {
            var response = await _bookService.AddBookAsync(book);
            return Ok(response);
        }

        // Cập nhật sách
        [HttpPut("{id}")]
        
        public async Task<IActionResult> Update(long id, [FromBody] BookDTO book)
        {
            var response = await _bookService.UpdateBookAsync(id, book);
            return Ok(response);
        }

        // Xóa sách (chuyển trạng thái INACTIVE)
        [HttpDelete("{id}")]
    
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _bookService.DeleteBookAsync(id);
            return Ok(response);
        }

        // Tìm kiếm theo tiêu đề
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string title)
        {
            var response = await _bookService.GetAllBookAsync();

            if (response.Data is IEnumerable<Book> books && !string.IsNullOrWhiteSpace(title))
            {
                var result = books.Where(b =>
                    b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
                return Ok(new ApiRespone { Succeeded = true, Message = "Search results", Data = result });
            }

            return Ok(response);
        }
    }
}

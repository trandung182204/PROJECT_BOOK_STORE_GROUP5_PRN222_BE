using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet("book/{bookId}/reviews")]
        public async Task<IActionResult> GetReviewByBook(long bookId)
        {

            return Ok(await _reviewService.GetReviewByBookIdAsync(bookId));
        }
        [HttpGet("user/{userId}/reviews")]
        public async Task<IActionResult> GetReviewByUser(string userId)
        {

            return Ok(await _reviewService.GetReviewByUserIdAsync(userId));
        }
        // phân quyền dữ liệu (thg user mua sách nào thì có thể review dc cuốn sách đó)
        [HttpPost("books/{bookId}/reviews")]
        public async Task<IActionResult> AddReview(long bookId, [FromBody] ReviewDto dto)
        {

<<<<<<< HEAD
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var result = await _reviewService.AddReviewAsync(bookId, userId, dto.rating, dto.comment);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);

=======
            return Ok(await _reviewService.AddReviewAsync(bookId, dto.UserId, dto.rating, dto.comment));
        
>>>>>>> developer
        }
        // tương tự
        [HttpPut("review/{id}")]
        public async Task<IActionResult> UpdateReview(long id, [FromBody] ReviewDto dto)
        {
            // get User from token login (JWT)
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");
            return Ok(await _reviewService.UpdateReviewAsync(id, dto.rating, dto.comment, userId));
        }
        [HttpDelete("review/{id}")]
        public async Task<IActionResult> DeleteReview(long id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");
            return Ok(await _reviewService.DeleteReviewAsync(id, userId));
        }
    }
}

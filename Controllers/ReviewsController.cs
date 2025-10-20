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
            var bookReview = await _reviewService.GetReviewByBookIdAsync(bookId);
            return Ok(bookReview);
        }
        [HttpGet("user/{userId}/reviews")]
        public async Task<IActionResult> GetReviewByUser(string userId)
        {
            var userReview = await _reviewService.GetReviewByUserIdAsync(userId);
            return Ok(userReview);
        }
        [HttpPost("books/{bookId}/reviews")]
        public async Task<IActionResult> AddReview(long bookId, [FromBody] ReviewDto dto)
        {
            var addReview = await _reviewService.AddReviewAsync(bookId, dto.UserId, dto.rating, dto.comment);
            return CreatedAtAction(nameof(GetReviewByBook), new { bookId = addReview!.BookId }, addReview);
        }
        [HttpPut("review/{id}")]
        public async Task<IActionResult> UpdateReview(long id, [FromBody] ReviewDto dto)
        {
            var review = await _reviewService.UpdateReviewAsync(id, dto.rating, dto.comment);
            if (review == null) return NotFound();
            return Ok(review);
        }
        [HttpDelete("review/{id}")]
        public async Task<IActionResult> DeleteReview(long id)
        {
            var delete = await _reviewService.DeleteReviewAsync(id);
            if (!delete) return NotFound();
            return NoContent();
        }
    }
}

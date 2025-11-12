using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GetReviewByUser(string userId)
        {

            return Ok(await _reviewService.GetReviewByUserIdAsync(userId));
        }
        // phân quyền dữ liệu (thg user mua sách nào thì có thể review dc cuốn sách đó)
        [HttpPost("books/{bookId}/reviews")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddReview(long bookId, [FromBody] ReviewDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var currentRole = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User not authenticated.");

            var result = await _reviewService.AddReviewAsync(bookId, currentUserId!, dto.rating, dto.comment);

            if (!result.Succeeded)
                return BadRequest(result);

            // tra ve ten cuon sach(sau khi review)
            // var book = await _reviewService.GetReviewByBookIdAsync(bookId);
            return Ok(result);


        }
        // tương tự
        [HttpPut("review/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(long id, [FromBody] ReviewDto dto)
        {
            // get User from token login (JWT)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            return Ok(await _reviewService.UpdateReviewAsync(id, dto.rating, dto.comment, currentUserId));
        }
        [HttpDelete("review/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(long id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            return Ok(await _reviewService.DeleteReviewAsync(id, currentUserId));
        }
    }
}

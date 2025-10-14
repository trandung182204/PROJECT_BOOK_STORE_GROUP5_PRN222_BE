using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var cart = await _cartService.AddCartAsync(request.UserId, request.BookId, request.Quantity);
            return Ok(new { message = "Book added to cart successfully!", cart });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCartItem(long id)
        {
            await _cartService.DeleteCartItemAsync(id);
            return Ok(new { message = "Delete cartItem successfully!", id });
        }
    }
}

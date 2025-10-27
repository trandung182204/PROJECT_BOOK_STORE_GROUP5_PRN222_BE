using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;
using System.Security.Claims;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartByUserIdAsync(GetUserId());
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var cart = await _cartService.AddCartAsync(GetUserId(), request.BookId, request.Quantity);
            return Ok(new { message = "Added to cart", cart });
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateQuantity(long itemId, [FromBody] UpdateQuantityRequest request)
        {
            var item = await _cartService.UpdateCartItemQuantityAsync(itemId, request.Quantity);
            return Ok(item);
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> DeleteItem(long itemId)
        {
            await _cartService.DeleteCartItemAsync(itemId);
            return Ok(new { message = "Deleted" });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(GetUserId());
            return Ok(new { message = "Cart cleared" });
        }
    }
}

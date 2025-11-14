using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels; // Đảm bảo bạn có using ViewModel
using System.Security.Claims;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET /api/checkout/summary (Giữ nguyên)
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _checkoutService.GetCheckoutSummaryAsync(GetUserId());
            return Ok(result);
        }

        // ✨ HỢP NHẤT API: Dùng 1 endpoint duy nhất
        // POST /api/checkout
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();

            if (string.Equals(request.PaymentMethod, "cod", System.StringComparison.OrdinalIgnoreCase))
            {
                // Xử lý COD
                var result = await _checkoutService.CreateOrderCODAsync(userId, request.ShippingAddress, request.Note);
                return Ok(result);
            }
            else if (string.Equals(request.PaymentMethod, "vnpay", System.StringComparison.OrdinalIgnoreCase))
            {
                // Xử lý VNPAY (Service đã được sửa ở dưới)
                var result = await _checkoutService.CreateVnPayPaymentUrlAsync(userId, request.ShippingAddress, request.Note);
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "Phương thức thanh toán không hợp lệ." });
            }
        }

        // GET /api/checkout/vnpay/return (Giữ nguyên)
        [AllowAnonymous]
        [HttpGet("vnpay/return")]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayReturnRequest request)
        {
            var result = await _checkoutService.HandleVnPayReturnAsync(request);

            // Bạn có thể muốn Redirect về trang Frontend sau khi xử lý
            // Ví dụ: return Redirect("http://link-frontend.com/thankyou");

            return Ok(result);
        }
    }
}
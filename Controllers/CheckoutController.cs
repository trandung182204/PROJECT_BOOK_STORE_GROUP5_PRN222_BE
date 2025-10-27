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
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _checkoutService.GetCheckoutSummaryAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost("cod")]
        public async Task<IActionResult> CheckoutCOD([FromBody] CheckoutRequest request)
        {
            var order = await _checkoutService.CreateOrderCODAsync(GetUserId(), request.ShippingAddress, request.Note);
            return Ok(new { message = "COD order created successfully", order });
        }

        [HttpPost("vnpay")]
        public async Task<IActionResult> CreateVnPayUrl()
        {
            var url = await _checkoutService.CreateVnPayPaymentUrlAsync(GetUserId());
            return Ok(new { paymentUrl = url });
        }

        [AllowAnonymous]
        [HttpGet("vnpay/return")]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayReturnRequest request)
        {
            var result = await _checkoutService.HandleVnPayReturnAsync(request);
            return Ok(result);
        }
    }
}

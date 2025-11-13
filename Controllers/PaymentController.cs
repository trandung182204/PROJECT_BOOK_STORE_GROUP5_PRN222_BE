using System.Security.Claims;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Get /api/payments
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPayments([FromQuery] string? type = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            if (currentRole != "Customer")
            {
                return Ok(new { message = "You are not allowed to access this payment." });
            }
            var payment = await _paymentService.GetPayments(type,from,to);
            return Ok(payment);
        }
        // POST /api/payments
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO payment)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            if (currentRole == null || currentUserId == null)
            {
                return Ok(new { message = "You are not allowed to access here." });
            }
            var created = await _paymentService.CreatePaymentAsync(payment);
            return Ok(new { message = "Payment created successfully!", payment = created });
        }

        // GET /api/payments/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentDetail(string id)
        {
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            if (!(currentRole == "Admin" || currentRole == "Staff"))
            {
                return Ok(new { message = "You are not allowed to access payments." });
            }
            else
            {
                var payment = await _paymentService.GetPaymentDetailAsync(id);
                if (payment == null)
                    return NotFound(new { message = "Payment not found!" });
                return Ok(payment);
            }
        }

        // POST /api/payments/webhook
        [HttpPost("webhook")]
        public async Task<IActionResult> PaymentWebhook()
        {
            // bỏ trống
            //using var reader = new StreamReader(Request.Body);
            //var payload = await reader.ReadToEndAsync();
            //await _paymentService.HandleWebhookAsync(payload);

            return Ok(new { message = "Webhook processed successfully" });
        }
    }
}

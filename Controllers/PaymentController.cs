using Azure.Core;
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

        // POST /api/payments
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
        {
            var created = await _paymentService.CreatePaymentAsync(payment);
            return Ok(new { message = "Payment created successfully!", payment = created });
        }

        // GET /api/payments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentDetail(long id)
        {
            var payment = await _paymentService.GetPaymentDetailAsync(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found!" });

            return Ok(payment);
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

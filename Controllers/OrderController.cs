using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    // thiếu phần dữ liệu view của admin
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // GET /api/orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders();
            return Ok(orders);
        }
        // GET /api/orders/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersOfUser(string userId)
        {
            //var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var currentRole = User.FindFirstValue(ClaimTypes.Role);
            //if(userId != currentUserId)
            //{
            //    return Ok(new { message = $"Invalid roll" });
            //}
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetail(string id)
        {
            var order = await _orderService.GetOrderDetailAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found!" });

            return Ok(order);
        }

        // PUT /api/orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            return Ok(new { message = $"Order {id} status updated to {status}" });
        }
    }
}

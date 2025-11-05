using System.Security.Claims;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetOrders([FromQuery] string? type = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var orders = await _orderService.GetOrders(type, from, to);
            return Ok(orders);
        }
        // GET /api/orders/{userId}
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetOrdersOfUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            Console.WriteLine(currentUserId +" va " +currentRole);
            // Nếu là Customer thì chỉ được xem đơn hàng của chính mình
            if (currentRole == "Customer" && userId != currentUserId)
            {
                return Ok(new { message = "You are not allowed to access other users' orders." });
            }

            // Nếu là Admin hoặc Staff thì được phép xem tất cả
            if (currentRole == "Admin" || currentRole == "Staff")
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
                    ////ewqeq
            }

            // Nếu không thuộc vai trò hợp lệ thì từ chối
            if (currentRole != "Customer")
            {
                return Ok(new { message = "Your role cannot access this resource." });
            }

            // Trường hợp customer xem đơn hàng của chính họ
            var userOrders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(userOrders);
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(string id)
        {
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            if (!(currentRole == "Admin" || currentRole == "Staff"))
            {
                return Ok(new { message = "You are not allowed to access orders." });
            }
            else
            {
                var order = await _orderService.GetOrderDetailAsync(id);
                if (order == null)
                    return NotFound(new { message = "Order not found!" });

                return Ok(order);
            }   
        }

        // PUT /api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string status)
        {
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            if(currentRole != "Staff") return StatusCode(403, new { message = "You are not allowed to access orders." });
            await _orderService.UpdateOrderStatusAsync(id, status);
            return Ok(new { message = $"Order {id} status updated to {status}" });
        }
    }
}

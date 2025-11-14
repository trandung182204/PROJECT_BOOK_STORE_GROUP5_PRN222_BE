using System.Security.Claims;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
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
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            Console.WriteLine(currentUserId + " va " + currentRole);
            var orders = await _orderService.GetOrders(type, from, to);
            return Ok(orders);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO order)
        {
            var orders = await _orderService.AddOrders(order);
            return Ok(orders);
        }
        // GET /api/orders/{userId}
        [HttpGet("user/{userId}")]
        //[Authorize]
        public async Task<IActionResult> GetOrdersOfUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);

            // Nếu là Admin thì được phép xem tất cả
            if (currentRole == "Admin")
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }

            // Nếu không thuộc vai trò hợp lệ thì từ chối
            if (currentRole != "Customer")
            {
                return Ok(new { message = "Your role cannot access this resource." });
            }
            // Trường hợp customer xem đơn hàng của chính họ
            var userOrders = await _orderService.GetOrdersByUserIdAsync(currentUserId);
            return Ok(userOrders);
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(string id)
        {
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            if (!(currentRole == "Admin" || currentRole == "Customer") )
            {
                return Ok(new { message = "You are not allowed to access orders." });
            }

            var order = await _orderService.GetOrderDetailAsync(id);
                if (order == null)
                    return NotFound(new { message = "Order not found!" });
                return Ok(order);
              
        }

        // PUT /api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string status)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentRole == "Customer" && status != "CANCELLED")
                return Ok( new { message = "Customer can only cancel order." });
            //if (currentRole != "Admin") 
            //    return Ok( new { message = "You are not allowed to access orders." });
            await _orderService.UpdateOrderStatusAsync(id, status);
            return Ok(new { message = $"Order {id} status updated to {status}" });
        }
    }
}

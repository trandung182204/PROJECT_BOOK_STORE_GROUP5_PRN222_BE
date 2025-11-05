using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly BookStoreContext _context;

        public OrderService(IOrderRepository orderRepository, BookStoreContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<ApiResponse> GetOrderDetailAsync(string id)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get order success",
                Data = await _orderRepository.GetOrderDetailAsync(id)
            };
        }

        public async Task<ApiResponse> GetOrders(string? type = null, DateTime? from = null, DateTime? to = null)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get order success",
                Data = await _orderRepository.GetOrders(type, from, to)
            };
        }

        public async Task<ApiResponse> GetOrdersByUserIdAsync(string userId)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get order success",
                Data = await _orderRepository.GetOrdersByUserIdAsync(userId)
            };
        }

        public async Task<ApiResponse> UpdateOrderStatusAsync(string id, string status)
        {
            if (!long.TryParse(id, out var orderId))
                throw new ArgumentException("Invalid order ID format.");

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found!");
            order.OrderStatus = status;
            await _orderRepository.UpdateAsync(order);
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Update success",
                Data = order
            };
        }
    }
}

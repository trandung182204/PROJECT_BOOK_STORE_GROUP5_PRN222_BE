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

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task<Order?> GetOrderDetailAsync(string id)
        {
            return await _orderRepository.GetOrderDetailAsync(id);
        }

        public async Task UpdateOrderStatusAsync(string id, string status)
        {
            if (!long.TryParse(id, out var orderId))
                throw new ArgumentException("Invalid order ID format.");

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found!");
            order.OrderStatus = status;
            await _orderRepository.UpdateAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _orderRepository.GetOrders();
        }

       
    }
}

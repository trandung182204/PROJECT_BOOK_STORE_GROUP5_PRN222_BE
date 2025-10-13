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

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(long userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task<Order?> GetOrderDetailAsync(long id)
        {
            return await _orderRepository.GetOrderDetailAsync(id);
        }

        public async Task UpdateOrderStatusAsync(long id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
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

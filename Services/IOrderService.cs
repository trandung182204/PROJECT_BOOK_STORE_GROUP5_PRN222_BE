using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrders();
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(long userId);
        Task<Order?> GetOrderDetailAsync(long id);
        Task UpdateOrderStatusAsync(long id, string status);
    }
}

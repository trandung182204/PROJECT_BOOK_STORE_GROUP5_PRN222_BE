using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IOrderService
    {
        Task<ApiResponse> GetOrders(string? type = null, DateTime? from = null, DateTime? to = null);
        Task<ApiResponse> GetOrdersByUserIdAsync(string userId);
        Task<ApiResponse> GetOrderDetailAsync(string id);
        Task<ApiResponse> UpdateOrderStatusAsync(string id, string status);
    }
}

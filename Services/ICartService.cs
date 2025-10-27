using System.Collections.Generic;
using System.Threading.Tasks;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface ICartService
    {
        Task<ApiResponse> GetCartByUserIdAsync(string userId);
        Task<ApiResponse> AddCartAsync(string userId, long bookId, int quantity);
        Task<ApiResponse> UpdateCartItemQuantityAsync(long itemId, int quantity);
        Task<ApiResponse> DeleteCartItemAsync(long itemId);
        Task<ApiResponse> ClearCartAsync(string userId);
    }
}

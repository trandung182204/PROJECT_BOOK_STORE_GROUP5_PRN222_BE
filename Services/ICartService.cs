using System.Collections.Generic;
using System.Threading.Tasks;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> AddCartAsync(string userId, long bookId, int quantity);
        Task<CartItem> UpdateCartItemQuantityAsync(long itemId, int quantity);
        Task DeleteCartItemAsync(long itemId);
        Task ClearCartAsync(string userId);
    }
}

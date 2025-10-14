using System.Collections.Generic;
using System.Threading.Tasks;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetAllCartAsync();
        Task<Cart> GetCartByIdAsync(long id);
        Task<Cart> AddCartAsync(string userId, long bookId, int quantity);
        Task UpdateCartAsync(Cart cart);
        Task DeleteCartAsync(long id);
        Task<Cart?> GetCartByUserId(string id);
        Task DeleteCartItemAsync(long cartItemId);
    }
}

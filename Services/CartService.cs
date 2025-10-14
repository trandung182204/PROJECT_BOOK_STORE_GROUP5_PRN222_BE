using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBaseRepository<CartItem> _cartItemRepository;
        private readonly BookStoreContext _bookStoreContext;

        public CartService(
            ICartRepository baseRepository,
            IBaseRepository<CartItem> cartItemRepository,
            BookStoreContext bookStoreContext)
        {
            _cartRepository = baseRepository;
            _cartItemRepository = cartItemRepository;
            _bookStoreContext = bookStoreContext;
        }

        public async Task<Cart> AddCartAsync(string userId, long bookId, int quantity)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                };
                await _cartRepository.AddAsync(cart);
                await _bookStoreContext.SaveChangesAsync();
            }

            var cartItem = await _bookStoreContext.CartItems
                .FirstOrDefaultAsync(x => x.BookId == bookId && x.CartId == cart.Id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    BookId = bookId,
                    Quantity = quantity
                };
                await _cartItemRepository.AddAsync(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                await _cartItemRepository.UpdateAsync(cartItem);
            }

            await _bookStoreContext.SaveChangesAsync();
            return cart;
        }

        public async Task DeleteCartAsync(long id)
        {
            var cart = await _cartRepository.GetByIdAsync(id);
            if (cart != null)
            {
                var cartItems = await _bookStoreContext.CartItems
                    .Where(ci => ci.CartId == id)
                    .ToListAsync();

                _bookStoreContext.CartItems.RemoveRange(cartItems);
                await _cartRepository.DeleteAsync(cart);
                await _bookStoreContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Cart>> GetAllCartAsync()
        {
            return await _bookStoreContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartByIdAsync(long id)
        {
            return await _bookStoreContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cart?> GetCartByUserId(string id)
        {
            return await _bookStoreContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            await _cartRepository.UpdateAsync(cart);
            await _bookStoreContext.SaveChangesAsync();
        }

        public async Task DeleteCartItemAsync(long cartItemId)
        {
            var cartItem = await _bookStoreContext.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemId);
            if(cartItem != null)
            {
                await _cartItemRepository.DeleteByIdAsync(cartItem.Id);
                await _bookStoreContext.SaveChangesAsync();
            }
        }
    }
}

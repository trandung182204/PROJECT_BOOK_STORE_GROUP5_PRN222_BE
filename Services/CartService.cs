using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

public class CartService : ICartService
{
    private readonly BookStoreContext _context;

    public CartService(BookStoreContext context)
    {
        _context = context;
    }

    public async Task<Cart> GetCartByUserIdAsync(string userId)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Book)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart> AddCartAsync(string userId, long bookId, int quantity)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var item = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == bookId);
        if (item == null)
        {
            _context.CartItems.Add(new CartItem { CartId = cart.Id, BookId = bookId, Quantity = quantity });
        }
        else
        {
            item.Quantity += quantity;
        }

        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartItem> UpdateCartItemQuantityAsync(long itemId, int quantity)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item == null) throw new Exception("Item not found");
        item.Quantity = quantity;
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task DeleteCartItemAsync(long itemId)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart != null)
        {
            var items = _context.CartItems.Where(ci => ci.CartId == cart.Id);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}

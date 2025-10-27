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

    public async Task<ApiResponse> GetCartByUserIdAsync(string userId)
    {
        return new ApiResponse
        {
            Succeeded = true,
            Message = "Get cart successfully",
            Data = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Book).FirstOrDefaultAsync(c => c.UserId == userId)
        };
    }

    public async Task<ApiResponse> AddCartAsync(string userId, long bookId, int quantity)
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
        return new ApiResponse
        {
            Succeeded = true,
            Message = "Add cart successfully",
            Data = cart
        };
    }

    public async Task<ApiResponse> UpdateCartItemQuantityAsync(long itemId, int quantity)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item == null)
            return new ApiResponse
            {
                Succeeded = false,
                Message = "invalid itemId",
            };

        item.Quantity = quantity;
        await _context.SaveChangesAsync();

        return new ApiResponse
        {
            Succeeded = true,
            Message = "Update cart item quantity successfully",
            Data = item
        };
    }

    public async Task<ApiResponse> DeleteCartItemAsync(long itemId)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        return new ApiResponse
        {
            Succeeded = true,
            Message = "Delete cart item successfully"
        };
    }

    public async Task<ApiResponse> ClearCartAsync(string userId)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart != null)
        {
            var items = _context.CartItems.Where(ci => ci.CartId == cart.Id);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
        return new ApiResponse
        {
            Succeeded = true,
            Message = "Clear cart successfully"
        };
    }
}

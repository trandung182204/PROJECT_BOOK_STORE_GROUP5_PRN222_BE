using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        private readonly BookStoreContext _bookStoreContext;

        public CartRepository(BookStoreContext context) : base(context)
        {
            _bookStoreContext = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await  _bookStoreContext.Carts.Include(c => c.CartItems)
                          .FirstOrDefaultAsync(x => x.Id.Equals(userId));
        }
    }
}

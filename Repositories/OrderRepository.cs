using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly BookStoreContext _context;

        public OrderRepository(BookStoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string username)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.User.FullName.Equals(username))
                .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailAsync(string id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id.Equals(id));
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _context.Orders
                .Include(_o => _o.OrderItems)
                .ThenInclude(_o => _o.Book)
                .ToListAsync();
        }

        
    }
}

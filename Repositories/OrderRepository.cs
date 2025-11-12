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

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userid)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .Include(o => o.User) 
                .Where(o => o.User.Id.Equals(userid))
                .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailAsync(string id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id.ToString().Equals(id));
        }

        public async Task<IEnumerable<Order>> GetOrders(string? orderStatus, DateTime? from, DateTime? to)
        {
            var query = _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Book)
                    .Include(o => o.User)
                    .AsQueryable();

            if (!string.IsNullOrEmpty(orderStatus))
            {
                query = query.Where(o => o.OrderStatus == orderStatus);
            }

            if (from.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= from.Value);
            }

            if (to.HasValue)
            {
                var endDate = to.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(o => o.CreatedAt <= endDate);
            }

            return await query.ToListAsync();
        }

        
    }
}

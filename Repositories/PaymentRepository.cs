using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        private readonly BookStoreContext _context;

        public PaymentRepository(BookStoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment?> GetPaymentDetailAsync(string id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id.ToString().Equals(id));
        }

        public async Task<IEnumerable<Payment>> GetPayments(string? paymentmethod, DateTime? from, DateTime? to)
        {
            var query = _context.Payments
                .Include(p => p.Order)
                .ThenInclude(o => o.User)
                .AsQueryable();
            if (!string.IsNullOrEmpty(paymentmethod))
            {
                query = query.Where(o => o.PaymentMethod == paymentmethod);
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

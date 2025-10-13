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

        public async Task<Payment?> GetPaymentDetailAsync(long id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}

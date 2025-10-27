using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        private new readonly BookStoreContext _context;
        public ReviewRepository(BookStoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewByBookIDAsync(long bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId && (r.IsDeleted == null || r.IsDeleted == false)).ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewByUserIDAsync(string userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId && (r.IsDeleted == null || r.IsDeleted == false)).ToListAsync();
        }
    }
}

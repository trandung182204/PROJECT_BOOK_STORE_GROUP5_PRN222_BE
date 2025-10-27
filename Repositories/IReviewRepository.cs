using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public interface IReviewRepository : IBaseRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewByBookIDAsync(long bookId);
        Task<IEnumerable<Review>> GetReviewByUserIDAsync(string userId);
    }
}

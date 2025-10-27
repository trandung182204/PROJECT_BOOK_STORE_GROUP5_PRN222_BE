using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetReviewByBookIdAsync(long bookId);
        Task<IEnumerable<Review>> GetReviewByUserIdAsync(string userId);
        Task<Review?> AddReviewAsync(long bookId, string userId, int rating, string comment);
        Task<Review?> UpdateReviewAsync(long id, int rating, string comment);
        Task<bool> DeleteReviewAsync(long id);
    }
}

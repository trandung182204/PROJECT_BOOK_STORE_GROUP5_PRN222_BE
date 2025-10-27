using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _review;
        private readonly BookStoreContext _bookStoreContext;
        private readonly IBaseRepository<Review> _baseRepository;
        public ReviewService(IReviewRepository review, BookStoreContext context, IBaseRepository<Review> baseRepository)
        {
            _review = review;
            _bookStoreContext = context;
            _baseRepository = baseRepository;
        }

        public async Task<Review?> AddReviewAsync(long bookId, string userId, int rating, string comment)
        {
            var review = new Review
            {
                BookId = bookId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };
            await _review.AddAsync(review);
            return review;
        }

        public async Task<bool> DeleteReviewAsync(long id)
        {
            var review = await _review.GetByIdAsync(id);
            if (review == null) return false;
            review.IsDeleted = true;
            await _review.UpdateAsync(review);
            return true;
        }

        public async Task<IEnumerable<Review>> GetReviewByBookIdAsync(long bookId)
        => await _review.GetReviewByBookIDAsync(bookId);

        public async Task<IEnumerable<Review>> GetReviewByUserIdAsync(string userId)
        => await _review.GetReviewByUserIDAsync(userId);

        public async Task<Review?> UpdateReviewAsync(long id, int rating, string comment)
        {
            var review = await _review.GetByIdAsync(id);
            if (review == null || review.IsDeleted == null) return null;
            review.Rating = rating;
            review.Comment = comment;
            await _review.UpdateAsync(review);
            return review;
        }

    }
}

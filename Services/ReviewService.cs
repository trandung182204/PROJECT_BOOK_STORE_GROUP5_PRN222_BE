using System.Net;
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

        public async Task<ApiResponse> AddReviewAsync(long bookId, string userId, int rating, string comment)
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
            return new ApiResponse
            {
                Succeeded = true,
                Message = "success",
                Data = review
            };
        }

        public async Task<ApiResponse> DeleteReviewAsync(long id)
        {
            var review = await _review.GetByIdAsync(id);
            if (review == null) return new ApiResponse
            {
                Succeeded = false,
                Message = "error",
                Data = review
            };
            review.IsDeleted = true;
            await _review.UpdateAsync(review);
            return new ApiResponse
            {
                Succeeded = true,
                Message = "success",
                Data = review
            };
        }

        public async Task<ApiResponse> GetReviewByBookIdAsync(long bookId)
        //=> await _review.GetReviewByBookIDAsync(bookId);
        {
            await _review.GetReviewByBookIDAsync(bookId);
            return new ApiResponse
            {
                Succeeded = true,
                Message = "success",
                Data = _review
            };
        }

        public async Task<ApiResponse> GetReviewByUserIdAsync(string userId)
        //=> await _review.GetReviewByUserIDAsync(userId);
        {
            await _review.GetReviewByUserIDAsync(userId);
            return new ApiResponse
            {
                Succeeded = true,
                Message = "success",
                Data = _review
    };
}

public async Task<ApiResponse> UpdateReviewAsync(long id, int rating, string comment)
        {
            var review = await _review.GetByIdAsync(id);
            if (review == null || review.IsDeleted == null) return new ApiResponse
            {
                Succeeded = false,
                Message = "not found",
                Data = review
            };
            review.Rating = rating;
            review.Comment = comment;
            await _review.UpdateAsync(review);
            return new ApiResponse
            {
                Succeeded = true,
                Message = "success",
                Data = review
            };
        }

    }
}

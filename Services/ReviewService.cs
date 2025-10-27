using System.Net;
using Microsoft.EntityFrameworkCore;
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
            // check user bought this book
            var HasPurchased = await _bookStoreContext.Orders
                .Include(o => o.OrderItems)
                .AnyAsync(o => o.UserId == userId
                && o.OrderStatus == "Completed" && o.OrderItems.Any(i => i.BookId == bookId));
            if (!HasPurchased)
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = "You can only review books you have successfully purchased."
                };
            }

            // Check user has reviewed
            var existingReview = await _bookStoreContext.Reviews
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
            if (existingReview != null)
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = "You have already reviewed this book"
                };
            }

            // add new review
            var review = new Review
            {
                BookId = bookId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };
            try
            {
                await _review.AddAsync(review); // repository AddAsync
                return new ApiResponse
                {
                    Succeeded = true,
                    Message = "Review added successfully.",
                    Data = review
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = $"Failed to add review: {ex.Message}"
                };
            }
            await _review.AddAsync(review);
            return new ApiResponse
            {
                Succeeded = true,
                Message = "success",
                Data = review
            };
        }

        public async Task<ApiResponse> DeleteReviewAsync(long id, string userId)
        {
            // Get review by Id
            var review = await _review.GetByIdAsync(id);
            if (review == null) return new ApiResponse
            {
                Succeeded = false,
                Message = "Review not found"
            };

            // User created the review can delete it
            if (review.UserId != userId)
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = "You are not authorized to delete this review."
                };
            }

            // delete soft
            review.IsDeleted = true;
            await _review.UpdateAsync(review);
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Review deleted successfully.",
                Data = review
            };
        }

        public async Task<ApiResponse> GetReviewByBookIdAsync(long bookId)
        //=> await _review.GetReviewByBookIDAsync(bookId);
        {
            var bookReviews = await _review.GetReviewByBookIDAsync(bookId);
            if (bookReviews == null || !bookReviews.Any())
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = $"Not found review by bookId = {bookId}"
                };
            }
            return new ApiResponse
            {
                Succeeded = true,
                Message = "success",
                Data = bookReviews
            };
        }

        public async Task<ApiResponse> GetReviewByUserIdAsync(string userId)
        //=> await _review.GetReviewByUserIDAsync(userId);
        {
            var bookReviews = await _review.GetReviewByUserIDAsync(userId);
            if (bookReviews == null || !bookReviews.Any())
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = $"Not found review by userID = {userId}"
                };
            }
            return new ApiResponse
            {
                Succeeded = true,
                Message = "success",
                Data = bookReviews
            };
        }

        public async Task<ApiResponse> UpdateReviewAsync(long id, int rating, string comment, string userId)
        {
            // get by Id
            var review = await _review.GetByIdAsync(id);

            // check exist and not deleted
            if (review == null || review.IsDeleted == null)
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = "Review not found",
                };
            }

            // User created the review can update it
            if (review.UserId != userId)
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = "You can't update this review."
                };
            }

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

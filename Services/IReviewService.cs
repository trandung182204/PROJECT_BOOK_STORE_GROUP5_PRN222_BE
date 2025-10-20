using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IReviewService
    {
        Task<ApiResponse> GetReviewByBookIdAsync(long bookId);
        Task<ApiResponse> GetReviewByUserIdAsync(string userId);
        Task<ApiResponse> AddReviewAsync(long bookId, string userId, int rating, string comment);
        Task<ApiResponse> UpdateReviewAsync(long id, int rating, string comment);
        Task<ApiResponse> DeleteReviewAsync(long id);
    }
    //public class ApiResponse
    //{
    //    public bool Succeeded { get; set; }
    //    public string Message { get; set; } = null!;
    //    public object Data { get; set; } = null!;
    //}
}

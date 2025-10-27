using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IPaymentService
    {
        Task<ApiResponse> CreatePaymentAsync(Payment payment);
        Task<ApiResponse?> GetPaymentDetailAsync(string id);
        Task<ApiResponse> HandleWebhookAsync(string payload);
    }
}

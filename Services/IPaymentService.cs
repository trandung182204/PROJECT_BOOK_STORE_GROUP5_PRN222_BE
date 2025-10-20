using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment?> GetPaymentDetailAsync(string id);
        Task HandleWebhookAsync(string payload);
    }
}

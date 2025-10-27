using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface ICheckoutService
    {
        Task<Order> CreateOrderCODAsync(string userId, string address, string note);
        Task<string> CreateVnPayPaymentUrlAsync(string userId);
        Task<object> HandleVnPayReturnAsync(VnPayReturnRequest request);
        Task<object> GetCheckoutSummaryAsync(string userId);
    }
}

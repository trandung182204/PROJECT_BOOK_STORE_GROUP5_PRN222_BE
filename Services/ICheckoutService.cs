using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface ICheckoutService
    {
        Task<ApiResponse> CreateOrderCODAsync(string userId, string address, string note);
        Task<ApiResponse> CreateVnPayPaymentUrlAsync(string userId);
        Task<ApiResponse> HandleVnPayReturnAsync(VnPayReturnRequest request);
        Task<ApiResponse> GetCheckoutSummaryAsync(string userId);
    }
}

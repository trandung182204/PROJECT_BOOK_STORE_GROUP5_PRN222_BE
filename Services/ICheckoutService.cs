using System.Threading.Tasks;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface ICheckoutService
    {
        Task<ApiResponse> GetCheckoutSummaryAsync(string userId);
        Task<ApiResponse> CreateOrderCODAsync(string userId, string address, string note);

        // ✨ SỬA LẠI HÀM NÀY: Phải nhận address và note
        Task<ApiResponse> CreateVnPayPaymentUrlAsync(string userId, string address, string note);

        Task<ApiResponse> HandleVnPayReturnAsync(VnPayReturnRequest request);
    }
}
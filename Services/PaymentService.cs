using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly BookStoreContext _context;

        public PaymentService(IPaymentRepository paymentRepository, BookStoreContext context)
        {
            _paymentRepository = paymentRepository;
            _context = context;
        }

        public async Task<ApiResponse> CreatePaymentAsync(Payment payment)
        {
            await _paymentRepository.AddAsync(payment);
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Create completed",
                Data = payment
            };
        }

        public async Task<ApiResponse?> GetPaymentDetailAsync(string id)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get payment detail success",
                Data = await _paymentRepository.GetPaymentDetailAsync(id)
            };
        }

        public async Task<ApiResponse> HandleWebhookAsync(string payload)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Success",
                Data = payload
            };
        }
    }
}

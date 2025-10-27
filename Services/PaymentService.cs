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

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _paymentRepository.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetPaymentDetailAsync(string id)
        {
            return await _paymentRepository.GetPaymentDetailAsync(id);
        }

        public async Task HandleWebhookAsync(string payload)
        {
            // bỏ trống chưa làm
        }
    }
}

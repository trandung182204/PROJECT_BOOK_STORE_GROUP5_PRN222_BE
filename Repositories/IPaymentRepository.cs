using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPayments(string? paymentmethod, DateTime? from, DateTime? to);
        Task<Payment?> GetPaymentDetailAsync(string id);
    }
}

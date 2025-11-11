using Microsoft.EntityFrameworkCore;
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

        public async Task<ApiRespone> CreatePaymentAsync(PaymentDTO payment)
        {
            try
            {
                if (payment == null)
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Payment cannot be null." });

                if (payment.OrderId <= 0)
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "OrderId is required and must be greater than 0." });

                if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Payment method is required." });

                if (payment.Amount <= 0)
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Payment amount must be greater than 0." });

                // Kiểm tra trùng giao dịch nếu có TransactionId
                if (!string.IsNullOrWhiteSpace(payment.TransactionId))
                {
                    bool exists = await _context.Payments
                        .AnyAsync(p => p.TransactionId == payment.TransactionId);
                    if (exists)
                        return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Transaction ID already exists." });
                }

                // Nếu status không có -> mặc định là "PENDING"
                payment.Status ??= "PENDING";

                var newPayment = new Payment
                {
                    OrderId = payment.OrderId,
                    PaymentMethod = payment.PaymentMethod?.Trim(),
                    Amount = payment.Amount,
                    Status = payment.Status?.Trim(),
                    TransactionId = payment.TransactionId?.Trim(),
                    PaidAt = payment.PaidAt,
                    CreatedAt = DateTime.Now
                };

                await _paymentRepository.AddAsync(newPayment);

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Payment added successfully.",
                    Data = newPayment
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone
                {
                    Succeeded = false,
                    Message = $"Error: {ex.Message}"
                };
            }
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

        public async Task<ApiResponse> GetPayments(string? method = null, DateTime? from = null, DateTime? to = null)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get payment success",
                Data = await _paymentRepository.GetPayments(method, from, to)
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
        public async Task<ApiRespone> UpdatePaymentAsync(PaymentDTO payment)
        {
            try
            {
                if (payment == null)
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Payment cannot be null." });

                if (payment.Id <= 0)
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Payment ID is invalid." });

                var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == payment.Id);
                if (existingPayment == null)
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Payment not found." });

                // Validate thông tin cơ bản
                if (payment.Amount <= 0)
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Amount must be greater than 0." });

                if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
                    return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Payment method is required." });

                // Nếu có TransactionId, kiểm tra trùng (ngoại trừ bản hiện tại)
                if (!string.IsNullOrWhiteSpace(payment.TransactionId))
                {
                    bool exists = await _context.Payments
                        .AnyAsync(p => p.TransactionId == payment.TransactionId && p.Id != payment.Id);
                    if (exists)
                        return await Task.FromResult(new ApiRespone { Succeeded = false, Message = "Transaction ID already exists." });
                }

                // Cập nhật thông tin
                existingPayment.OrderId = payment.OrderId > 0 ? payment.OrderId : existingPayment.OrderId;
                existingPayment.PaymentMethod = payment.PaymentMethod?.Trim() ?? existingPayment.PaymentMethod;
                existingPayment.Amount = payment.Amount;
                existingPayment.Status = payment.Status?.Trim() ?? existingPayment.Status;
                existingPayment.TransactionId = payment.TransactionId?.Trim() ?? existingPayment.TransactionId;
                existingPayment.PaidAt = payment.PaidAt ?? existingPayment.PaidAt;
                existingPayment.CreatedAt = existingPayment.CreatedAt; // giữ nguyên ngày tạo

                _context.Payments.Update(existingPayment);
                await _context.SaveChangesAsync();

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Payment updated successfully.",
                    Data = existingPayment
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone
                {
                    Succeeded = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}

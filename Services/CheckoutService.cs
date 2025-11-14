using System;
using System.Security.Policy;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;
using System.Linq; // ✨ Thêm using này
using System.Threading.Tasks; // ✨ Thêm using này

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly BookStoreContext _context;
        private readonly IConfiguration _config;
        private readonly IVnPayService _vnPayService;

        public CheckoutService(BookStoreContext context, IConfiguration config, IVnPayService vnPayService)
        {
            _context = context;
            _config = config;
            _vnPayService = vnPayService;
        }

        public async Task<ApiResponse> GetCheckoutSummaryAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            decimal subtotal = cart.CartItems.Sum(i => i.Book.Price * i.Quantity);
            decimal shipping = subtotal > 500000 ? 0 : 20000;
            decimal total = subtotal + shipping;

            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get checkout summary successful!",
                Data = new
                {
                    Subtotal = subtotal,
                    Shipping = shipping,
                    Total = total
                }
            };
        }

        public async Task<ApiResponse> CreateOrderCODAsync(string userId, string address, string note)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            decimal total = cart.CartItems.Sum(i => i.Book.Price * i.Quantity);
            decimal shipping = total >= 300 ? 0 : 15;
            decimal grandTotal = total + shipping;

            var order = new Order
            {
                UserId = userId,
                TotalAmount = grandTotal, // ✨ Sửa: dùng grandTotal
                ShippingFee = shipping,   // ✨ Sửa: dùng shipping
                ShippingAddress = address,
                PaymentMethod = "COD",
                PaymentStatus = "PENDING",
                OrderStatus = "SHIPPING" // ✨ Thêm trạng thái đơn hàng
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart.CartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Book.Price
                });
            }

            await _context.SaveChangesAsync();
            await ClearCartAsync(cart); // Xóa giỏ hàng

            return new ApiResponse
            {
                Succeeded = true,
                Message = "Create Order COD successful!",
                Data = order
            };
        }

        // ✨ ============ SỬA LẠI HOÀN TOÀN HÀM NÀY ============
        public async Task<ApiResponse> CreateVnPayPaymentUrlAsync(string userId, string address, string note)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            decimal total = cart.CartItems.Sum(i => i.Book.Price * i.Quantity);
            decimal shipping = total >= 300 ? 0 : 15;
            decimal grandTotal = total + shipping;

            var order = new Order
            {
                UserId = userId,
                TotalAmount = grandTotal,
                ShippingFee = shipping,
                ShippingAddress = address, // ✨ LƯU ĐỊA CHỈ
                PaymentMethod = "VNPAY",
                PaymentStatus = "PENDING",
                OrderStatus = "PENDING_PAYMENT", // Trạng thái chờ thanh toán
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Lưu để lấy OrderId

            // ✨ THÊM ORDER ITEMS (Giống hàm COD)
            foreach (var item in cart.CartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Book.Price
                });
            }

            // ✨ QUAN TRỌNG: KHÔNG XÓA GIỎ HÀNG VỘI!
            await _context.SaveChangesAsync(); // Lưu OrderItems

            // Tạo link VNPAY
            string url = _vnPayService.CreatePaymentUrl(order.Id.ToString(), grandTotal, $"Thanh toan don hang #{order.Id}");

            return new ApiResponse
            {
                Succeeded = true,
                Message = "Create VNPAY payment url successful!",
                Data = new { paymentUrl = url } // ✨ Đóng gói URL vào object
            };
        }

        // ✨ ============ SỬA HÀM NÀY ĐỂ XÓA GIỎ HÀNG ============
        public async Task<ApiResponse> HandleVnPayReturnAsync(VnPayReturnRequest request)
        {
            long orderId = long.Parse(request.vnp_TxnRef);
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return new ApiResponse { Succeeded = false, Message = "Order invalid" };

            // (Giừng nguyên logic ValidateSignature)
            var responseDict = request.GetType()
                .GetProperties()
                .Where(p => p.Name.StartsWith("vnp_"))
                .ToDictionary(p => p.Name, p => p.GetValue(request)?.ToString() ?? "");
            bool valid = _vnPayService.ValidateSignature(responseDict, request.vnp_SecureHash);

            if (!valid)
                return new ApiResponse { Succeeded = false, Message = "Invalid Signature" };

            if (request.vnp_ResponseCode == "00")
            {
                // Thanh toán thành công
                order.PaymentStatus = "PAID";
                order.OrderStatus = "WAIT_CONFIRM";
                order.UpdatedAt = DateTime.Now;

                _context.Payments.Add(new Payment
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    Status = "SUCCESS",
                    PaymentMethod = "VNPAY",
                    TransactionId = request.vnp_TxnRef,
                    PaidAt = DateTime.Now
                });

                // ✨ XÓA GIỎ HÀNG SAU KHI THANH TOÁN THÀNH CÔNG
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == order.UserId);
                if (cart != null)
                {
                    await ClearCartAsync(cart); // Gọi hàm ClearCart của bạn
                }

                await _context.SaveChangesAsync();
                return new ApiResponse { Succeeded = true, Message = "Create Payment Successful" };
            }

            // Thanh toán thất bại
            order.PaymentStatus = "FAILED";
            await _context.SaveChangesAsync();
            return new ApiResponse { Succeeded = false, Message = "Payment failed" };
        }

        private async Task ClearCartAsync(Cart cart)
        {
            var items = _context.CartItems.Where(ci => ci.CartId == cart.Id);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
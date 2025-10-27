using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels;
using System.Web;

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

        public async Task<object> GetCheckoutSummaryAsync(string userId)
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

            return new
            {
                Subtotal = subtotal,
                Shipping = shipping,
                Total = total
            };
        }

        public async Task<Order> CreateOrderCODAsync(string userId, string address, string note)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            decimal total = cart.CartItems.Sum(i => i.Book.Price * i.Quantity);

            var order = new Order
            {
                UserId = userId,
                TotalAmount = total,
                ShippingFee = total > 500000 ? 0 : 20000,
                ShippingAddress = address,
                PaymentMethod = "COD",
                PaymentStatus = "PENDING"
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
            await ClearCartAsync(cart);

            return order;
        }

        public async Task<string> CreateVnPayPaymentUrlAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            decimal total = cart.CartItems.Sum(i => i.Book.Price * i.Quantity);
            decimal shipping = total > 500000 ? 0 : 20000;
            decimal grandTotal = total + shipping;

            var order = new Order
            {
                UserId = userId,
                TotalAmount = grandTotal,
                PaymentMethod = "VNPAY",
                PaymentStatus = "PENDING",
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            string url = _vnPayService.CreatePaymentUrl(order.Id.ToString(), grandTotal, $"Thanh toan don hang #{order.Id}");
            return url;
        }

        public async Task<object> HandleVnPayReturnAsync(VnPayReturnRequest request)
        {
            long orderId = long.Parse(request.vnp_TxnRef);
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return new { success = false, message = "Order not found" };

            var responseDict = request.GetType()
                .GetProperties()
                .Where(p => p.Name.StartsWith("vnp_"))
                .ToDictionary(p => p.Name, p => p.GetValue(request)?.ToString() ?? "");

            bool valid = _vnPayService.ValidateSignature(responseDict, request.vnp_SecureHash);

            if (!valid)
                return new { success = false, message = "Sai chữ ký từ VNPAY" };

            if (request.vnp_ResponseCode == "00")
            {
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

                await _context.SaveChangesAsync();
                return new { success = true, message = "Payment successful", orderId = order.Id };
            }

            order.PaymentStatus = "FAILED";
            await _context.SaveChangesAsync();
            return new { success = false, message = "Payment failed" };
        }

        private async Task ClearCartAsync(Cart cart)
        {
            var items = _context.CartItems.Where(ci => ci.CartId == cart.Id);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}

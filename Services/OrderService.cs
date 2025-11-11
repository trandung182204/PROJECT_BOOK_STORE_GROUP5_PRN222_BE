using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly BookStoreContext _context;

        public OrderService(IOrderRepository orderRepository, BookStoreContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<ApiRespone> AddOrders(OrderDTO order)
        {
            try
            {
                if (order == null)
                    return new ApiRespone { 
                        Succeeded = false, 
                        Message = "Book cannot be null." 
                    };

                if (string.IsNullOrWhiteSpace(order.UserId))
                    return new ApiRespone { Succeeded = false, Message = "UserId is required." };

                if (string.IsNullOrWhiteSpace(order.OrderStatus))
                    order.OrderStatus = "PENDING"; // mặc định

                if (string.IsNullOrWhiteSpace(order.PaymentStatus))
                    order.PaymentStatus = "UNPAID"; // mặc định

                if (string.IsNullOrWhiteSpace(order.PaymentMethod))
                    return new ApiRespone { Succeeded = false, Message = "Payment method is required." };

                if (string.IsNullOrWhiteSpace(order.ShippingAddress))
                    return new ApiRespone { Succeeded = false, Message = "Shipping address is required." };

                if (order.TotalAmount <= 0)
                    return new ApiRespone { Succeeded = false, Message = "Total amount must be greater than 0." };

                if (order.ShippingFee < 0)
                    return new ApiRespone { Succeeded = false, Message = "Shipping fee cannot be negative." };

                // Nếu muốn kiểm tra trùng đơn hàng theo Id
                var exists = await _context.Orders.AnyAsync(o => o.Id == order.Id);
                if (exists)
                    return new ApiRespone { Succeeded = false, Message = "Order with this ID already exists." };

                var newOrder = new Order
                {
                    UserId = order.UserId?.Trim(),
                    OrderStatus = order.OrderStatus?.Trim(),
                    PaymentStatus = order.PaymentStatus?.Trim(),
                    PaymentMethod = order.PaymentMethod?.Trim(),
                    ShippingAddress = order.ShippingAddress?.Trim(),
                    ShippingFee = order.ShippingFee ?? 0,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _orderRepository.AddAsync(newOrder);

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Order added successfully.",
                    Data = newOrder
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone
                {
                    Succeeded = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse> GetOrderDetailAsync(string id)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get order success",
                Data = await _orderRepository.GetOrderDetailAsync(id)
            };
        }

        public async Task<ApiResponse> GetOrders(string? type = null, DateTime? from = null, DateTime? to = null)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get order success",
                Data = await _orderRepository.GetOrders(type, from, to)
            };
        }

        public async Task<ApiResponse> GetOrdersByUserIdAsync(string userId)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Get order success",
                Data = await _orderRepository.GetOrdersByUserIdAsync(userId)
            };
        }

        public async Task<ApiResponse> UpdateOrderStatusAsync(string id, string status)
        {
            if (!long.TryParse(id, out var orderId))
                throw new ArgumentException("Invalid order ID format.");

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found!");
            order.OrderStatus = status;
            await _orderRepository.UpdateAsync(order);
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                Succeeded = true,
                Message = "Update success",
                Data = order
            };
        }
    }
}

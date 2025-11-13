using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Lấy các giá trị từ appsettings.json
        private readonly string _baseUrl;
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _returnUrl;

        public VnPayService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;

            _baseUrl = _config["VnPay:BaseUrl"];
            _tmnCode = _config["VnPay:TmnCode"];
            _hashSecret = _config["VnPay:HashSecret"];
            _returnUrl = _config["VnPay:ReturnUrl"];
        }

        public string CreatePaymentUrl(string orderId, decimal amount, string orderInfo)
        {
            // 1. Lấy thông tin cần thiết
            var ipAddress = GetIpAddress(_httpContextAccessor.HttpContext);
            var now = DateTime.Now;

            // 2. Tạo SortedDictionary để tự động sắp xếp theo Alphabet
            var data = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _tmnCode },
                { "vnp_Amount", ((long)(amount * 100)).ToString() }, // ✨ Quan trọng: * 100
                { "vnp_CreateDate", now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", ipAddress },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" }, // Có thể tùy chỉnh
                { "vnp_ReturnUrl", _returnUrl },
                { "vnp_TxnRef", orderId }, // ID đơn hàng duy nhất
                // { "vnp_HashType", "SHA512" } // VNPAY mặc định là SHA512 nếu bỏ trống
            };

            // 3. Tạo chuỗi hashData (KHÔNG UrlEncode)
            var hashDataBuilder = new StringBuilder();
            foreach (var kvp in data)
            {
                hashDataBuilder.Append(kvp.Key);
                hashDataBuilder.Append('=');
                hashDataBuilder.Append(kvp.Value); // Giá trị gốc
                hashDataBuilder.Append('&');
            }
            string hashData = hashDataBuilder.ToString().TrimEnd('&');

            // 4. Tạo chữ ký
            string vnp_SecureHash = HmacSHA512(_hashSecret, hashData);

            // 5. Tạo URL cuối cùng (UrlEncode các giá trị)
            var queryBuilder = new StringBuilder();
            foreach (var kvp in data)
            {
                queryBuilder.Append(kvp.Key);
                queryBuilder.Append('=');
                queryBuilder.Append(Uri.EscapeDataString(kvp.Value)); // ✨ Quan trọng: UrlEncode giá trị
                queryBuilder.Append('&');
            }
            queryBuilder.Append("vnp_SecureHash=");
            queryBuilder.Append(Uri.EscapeDataString(vnp_SecureHash));

            return $"{_baseUrl}?{queryBuilder.ToString()}";
        }

        public bool ValidateSignature(Dictionary<string, string> responseData, string vnp_SecureHash)
        {
            // Lọc và sắp xếp các tham số trả về
            var data = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var kvp in responseData)
            {
                if (!string.IsNullOrEmpty(kvp.Value) && kvp.Key.StartsWith("vnp_"))
                {
                    data.Add(kvp.Key, kvp.Value);
                }
            }

            // Bỏ qua vnp_SecureHash và vnp_SecureHashType
            data.Remove("vnp_SecureHash");
            data.Remove("vnp_SecureHashType");

            // Tạo chuỗi hashData
            var hashDataBuilder = new StringBuilder();
            foreach (var kvp in data)
            {
                hashDataBuilder.Append(kvp.Key);
                hashDataBuilder.Append('=');
                hashDataBuilder.Append(kvp.Value);
                hashDataBuilder.Append('&');
            }
            string hashData = hashDataBuilder.ToString().TrimEnd('&');

            // Tạo chữ ký từ dữ liệu trả về
            string calculatedHash = HmacSHA512(_hashSecret, hashData);

            // So sánh với chữ ký VNPAY gửi về
            return calculatedHash.Equals(vnp_SecureHash, StringComparison.Ordinal);
        }

        // ============ HÀM HỖ TRỢ ============

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }

        private string GetIpAddress(HttpContext context)
        {
            // Cố gắng lấy IP public từ "X-Forwarded-For" (nếu có proxy)
            string ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress?.ToString();
            }

            // Xử lý IP localhost (::1 hoặc 127.0.0.1) khi test ở local
            if (ip == "::1" || ip == "127.0.0.1" || string.IsNullOrEmpty(ip))
            {
                // Thay thế bằng IP public (chỉ dùng cho test - VNPAY không chấp nhận localhost)
                // Bạn có thể lấy IP public của mình trên trang: https://www.whatismyip.com/
                // Hoặc chúng ta lấy IP nội bộ của máy
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        socket.Connect("8.8.8.8", 65530); // Kết nối (ảo) đến Google DNS
                        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                        ip = endPoint?.Address?.ToString();
                    }
                }
                catch (Exception)
                {
                    ip = "127.0.0.1"; // Nếu vẫn thất bại
                }
            }

            // Nếu có nhiều IP (do proxy), chỉ lấy IP đầu tiên
            if (ip != null && ip.Contains(","))
            {
                ip = ip.Split(',')[0].Trim();
            }

            return ip;
        }
    }
}
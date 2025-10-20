using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(string orderId, decimal amount, string orderInfo);
        bool ValidateSignature(IDictionary<string, string> responseData, string receivedHash);
    }

    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(string orderId, decimal amount, string orderInfo)
        {
            string baseUrl = _config["VnPay:BaseUrl"];
            string tmnCode = _config["VnPay:TmnCode"];
            string hashSecret = _config["VnPay:HashSecret"];
            string returnUrl = _config["VnPay:ReturnUrl"];

            var vnpParams = new SortedList<string, string>(StringComparer.Ordinal)
    {
        { "vnp_Version", "2.1.0" },
        { "vnp_Command", "pay" },
        { "vnp_TmnCode", tmnCode },
        { "vnp_Amount", ((int)(amount * 100)).ToString() },
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
        { "vnp_CurrCode", "VND" },
        { "vnp_IpAddr", "127.0.0.1" },
        { "vnp_Locale", "vn" },
        { "vnp_OrderInfo", orderInfo },
        { "vnp_OrderType", "other" },
        { "vnp_ReturnUrl", returnUrl },
        { "vnp_TxnRef", orderId }
    };

            // ⚠️ Bước QUAN TRỌNG: thêm loại hash (VNPAY yêu cầu có)
            vnpParams.Add("vnp_SecureHashType", "HMACSHA512");

            // 🔹 Bước 1: tạo rawData (chưa encode)
            string rawData = string.Join("&", vnpParams
                .Where(x => x.Key != "vnp_SecureHashType" && x.Key != "vnp_SecureHash")
                .Select(kv => $"{kv.Key}={kv.Value}"));

            // 🔹 Bước 2: tạo secure hash
            string secureHash = HmacSHA512(hashSecret, rawData);

            // 🔹 Bước 3: tạo query string (đã encode value)
            string query = string.Join("&", vnpParams
                .Where(x => x.Key != "vnp_SecureHash")
                .Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));

            // ✅ Thêm hash vào cuối cùng
            string paymentUrl = $"{baseUrl}?{query}&vnp_SecureHash={secureHash}";

            Console.WriteLine("======== RAW DATA (HASH INPUT) ========");
            Console.WriteLine(rawData);
            Console.WriteLine("=======================================");
            Console.WriteLine("HASH SECRET: " + hashSecret);
            Console.WriteLine("SECURE HASH: " + secureHash);
            Console.WriteLine("FULL PAYMENT URL: " + paymentUrl);

            return paymentUrl;
        }


        public bool ValidateSignature(IDictionary<string, string> responseData, string receivedHash)
        {
            string hashSecret = _config["VnPay:HashSecret"];
            var sorted = new SortedList<string, string>(responseData, StringComparer.Ordinal);

            // Bỏ 2 trường này khi hash
            sorted.Remove("vnp_SecureHash");
            sorted.Remove("vnp_SecureHashType");

            string rawData = string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));
            string computedHash = HmacSHA512(hashSecret, rawData);

            return string.Equals(receivedHash, computedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string HmacSHA512(string key, string input)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }
    }
}

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(string orderId, decimal amount, string orderInfo);
        bool ValidateSignature(Dictionary<string, string> responseData, string vnp_SecureHash);
    }
}
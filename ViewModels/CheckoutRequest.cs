namespace PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels
{
    public class CheckoutRequest
    {
        public string PaymentMethod { get; set; }  // COD / VNPAY
        public string ShippingAddress { get; set; }
        public string? Note { get; set; }
    }
}

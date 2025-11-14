namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models
{
    public class PaymentDTO
    {
        public long Id { get; set; } = 0!;

        public long OrderId { get; set; }

        public string? PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public string? Status { get; set; }

        public string? TransactionId { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}

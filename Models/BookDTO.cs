namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models
{
    public class BookDTO
    {
        public string Code { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Author { get; set; }

        public string? Publisher { get; set; }

        public string? Isbn { get; set; }

        public int? PublicationYear { get; set; }

        public int? PageCount { get; set; }

        public string? Language { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int? StockQuantity { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? Status { get; set; }
    }
}

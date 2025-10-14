namespace PROJECT_BOOK_STORE_GROUP5_PRN222.ViewModels
{
    public class AddToCartRequest
    {
        public string UserId { get; set; }
        public long BookId { get; set; }
        public int Quantity { get; set; }
    }
}

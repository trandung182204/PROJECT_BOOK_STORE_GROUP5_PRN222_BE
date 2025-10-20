namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models
{
    public class ApiRespone
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = null!;
        public object Data { get; set; } = null!;
    }
}

using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        // Tìm sách theo tên hoặc mã
        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        // Lấy sách theo tác giả
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);

        // Lấy sách theo khoảng giá
        Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // Lấy sách theo category
        Task<IEnumerable<Book>> GetBooksByCategoryIdAsync(long categoryId);
    }
}

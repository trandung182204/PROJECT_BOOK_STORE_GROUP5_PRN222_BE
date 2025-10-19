using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly BookStoreContext _bookStoreContext;
        public BookRepository(BookStoreContext context) : base(context)
        {
            _bookStoreContext = context;
        }
        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            return await _bookStoreContext.Books
                .Where(b => b.IsDeleted == false &&
                            (b.Title.Contains(keyword) || b.Code.Contains(keyword)))
                .ToListAsync();
        }

        // Lấy sách theo tác giả
        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
        {
            return await _bookStoreContext.Books
                .Where(b => b.IsDeleted == false && b.Author.Contains(author))
                .ToListAsync();
        }

        // Lấy sách theo khoảng giá
        public async Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _bookStoreContext.Books
                .Where(b => b.IsDeleted == false && b.Price >= minPrice && b.Price <= maxPrice)
                .ToListAsync();
        }

        // Lấy sách theo category (giả sử Book có navigation property BookCategories -> Category)
        public async Task<IEnumerable<Book>> GetBooksByCategoryIdAsync(long categoryId)
        {
            return await _bookStoreContext.Books
                .Include(b => b.BookCategories)
                .Where(b => b.BookCategories.Any(c => c.CategoryId == categoryId) && b.IsDeleted == false)
                .ToListAsync();
        }

    }
}

using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(long id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(long id);
    }
}

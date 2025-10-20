using System.Collections.Generic;
using System.Threading.Tasks;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IBookService
    {
        Task<ApiRespone> AddBookAsync(BookDTO book);
        Task<ApiRespone> UpdateBookAsync(long id, BookDTO book);
        Task<ApiRespone> DeleteBookAsync(long id);
        Task<ApiRespone> GetAllBookAsync();
        Task<ApiRespone> GetBookByIdAsync(long id);
    }
}

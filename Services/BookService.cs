using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class BookService : IBookService
    {
        private readonly IBaseRepository<Book> baseRepository;

        public BookService(IBaseRepository<Book> baseRepository)
        {
            this.baseRepository = baseRepository;
        }

        public async Task AddBookAsync(Book book)
        {
            // Validate input
            if (book == null)
                throw new ArgumentNullException(nameof(book), "Book cannot be null.");

            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title cannot be empty.");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Book author cannot be empty.");

            if (book.Price <= 0)
                throw new ArgumentException("Book price must be greater than 0.");

            if (book.StockQuantity < 0)
                throw new ArgumentException("Book quantity cannot be negative.");

            await baseRepository.AddAsync(book);
        }

        public async Task DeleteBookAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid book ID.");

            var book = await baseRepository.GetByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found.");

            book.Status = "INACTIVE";
            await baseRepository.UpdateAsync(book);
        }

        public async Task<IEnumerable<Book>> GetAllBookAsync()
        {
            var books = await baseRepository.GetAllAsync();
            if (books == null || !books.Any())
                throw new InvalidOperationException("No books available.");

            return books;
        }

        public async Task<Book?> GetBookByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid book ID.");

            var book = await baseRepository.GetByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found.");

            return book;
        }

        public async Task UpdateBookAsync(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book), "Book cannot be null.");

            if (book.Id <= 0)
                throw new ArgumentException("Invalid book ID.");

            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title cannot be empty.");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Book author cannot be empty.");

            if (book.Price <= 0)
                throw new ArgumentException("Book price must be greater than 0.");

            if (book.StockQuantity < 0)
                throw new ArgumentException("Book quantity cannot be negative.");

            var existing = await baseRepository.GetByIdAsync(book.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Book with ID {book.Id} not found.");

            await baseRepository.UpdateAsync(book);
        }
    }
}

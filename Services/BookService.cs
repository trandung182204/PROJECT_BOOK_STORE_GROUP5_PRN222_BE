    using Microsoft.EntityFrameworkCore;
    using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
    using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

    namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
    {
        public class BookService : IBookService
        {
            private readonly IBaseRepository<Book> _baseRepository;
            private readonly BookStoreContext _context;

            public BookService(IBaseRepository<Book> baseRepository, BookStoreContext context)
            {
                _baseRepository = baseRepository;
                _context = context;
            }

            // ✅ Thêm sách mới
            public async Task<ApiRespone> AddBookAsync(BookDTO book)
            {
                try
                {
                    if (book == null)
                        return new ApiRespone { Succeeded = false, Message = "Book cannot be null." };
                    if (string.IsNullOrWhiteSpace(book.Code))
                        return new ApiRespone { Succeeded = false, Message = "Book code is required." };
                    if (string.IsNullOrWhiteSpace(book.Title))
                        return new ApiRespone { Succeeded = false, Message = "Book title is required." };
                    if (string.IsNullOrWhiteSpace(book.Author))
                        return new ApiRespone { Succeeded = false, Message = "Book author is required." };
                    if (book.Price <= 0)
                        return new ApiRespone { Succeeded = false, Message = "Book price must be greater than 0." };
                    if (book.StockQuantity < 0)
                        return new ApiRespone { Succeeded = false, Message = "Book quantity cannot be negative." };

                    // Kiểm tra trùng code
                    var exists = await _context.Books.AnyAsync(b => b.Code == book.Code && b.IsDeleted == false);
                    if (exists)
                        return new ApiRespone { Succeeded = false, Message = "Book code already exists." };

                    var newBook = new Book
                    {
                        Code = book.Code.Trim(),
                        Title = book.Title.Trim(),
                        Author = book.Author?.Trim(),
                        Publisher = book.Publisher,
                        Isbn = book.Isbn,
                        PublicationYear = book.PublicationYear,
                        PageCount = book.PageCount,
                        Language = book.Language,
                        Description = book.Description,
                        Price = book.Price,
                        DiscountPrice = book.DiscountPrice,
                        StockQuantity = book.StockQuantity,
                        ThumbnailUrl = book.ThumbnailUrl,
                        Status = book.Status ?? "ACTIVE",
                        IsDeleted = false
                    };

                    await _baseRepository.AddAsync(newBook);

                    return new ApiRespone
                    {
                        Succeeded = true,
                        Message = "Book added successfully.",
                        Data = newBook
                    };
                }
                catch (Exception ex)
                {
                    return new ApiRespone { Succeeded = false, Message = $"Error: {ex.Message}", Data = null };
                }
            }

            // ✅ Cập nhật sách
            public async Task<ApiRespone> UpdateBookAsync(long id, BookDTO book)
            {
                try
                {
                    if (book == null)
                        return new ApiRespone { Succeeded = false, Message = "Book cannot be null." };

                    var existing = await _baseRepository.GetByIdAsync(id);
                    if (existing == null || existing.IsDeleted == true)
                        return new ApiRespone { Succeeded = false, Message = "Book not found." };

                    // Kiểm tra trùng code (trừ chính nó)
                    var exists = await _context.Books.AnyAsync(b => b.Code == book.Code && b.Id != id && b.IsDeleted == false);
                    if (exists)
                        return new ApiRespone { Succeeded = false, Message = "Book code already exists." };

                    existing.Code = book.Code.Trim();
                    existing.Title = book.Title.Trim();
                    existing.Author = book.Author?.Trim();
                    existing.Publisher = book.Publisher;
                    existing.Isbn = book.Isbn;
                    existing.PublicationYear = book.PublicationYear;
                    existing.PageCount = book.PageCount;
                    existing.Language = book.Language;
                    existing.Description = book.Description;
                    existing.Price = book.Price;
                    existing.DiscountPrice = book.DiscountPrice;
                    existing.StockQuantity = book.StockQuantity;
                    existing.ThumbnailUrl = book.ThumbnailUrl;
                    existing.Status = book.Status ?? existing.Status;
                    existing.UpdatedAt = DateTime.Now;

                    await _baseRepository.UpdateAsync(existing);

                    return new ApiRespone
                    {
                        Succeeded = true,
                        Message = "Book updated successfully.",
                        Data = existing
                    };
                }
                catch (Exception ex)
                {
                    return new ApiRespone { Succeeded = false, Message = $"Error: {ex.Message}", Data = null };
                }
            }

            // ✅ Xóa (mềm) sách
            public async Task<ApiRespone> DeleteBookAsync(long id)
            {
                try
                {
                    var book = await _baseRepository.GetByIdAsync(id);
                    if (book == null)
                        return new ApiRespone { Succeeded = false, Message = "Book not found." };
                if (book.IsDeleted == true && book.Status == "INACTIVE")
                    return new ApiRespone { Succeeded = false, Message = "Book was deleted." };

                book.IsDeleted = true;
                    book.Status = "INACTIVE";
                    book.UpdatedAt = DateTime.Now;

                    await _baseRepository.UpdateAsync(book);

                    return new ApiRespone
                    {
                        Succeeded = true,
                        Message = "Book deleted successfully.",
                        Data = book
                    };
                }
                catch (Exception ex)
                {
                    return new ApiRespone { Succeeded = false, Message = $"Error: {ex.Message}", Data = null };
                }
            }

            // ✅ Lấy tất cả sách
            public async Task<ApiRespone> GetAllBookAsync()
            {
                try
                {
                    var books = await _baseRepository.GetAllAsync();
                    var activeBooks = books.Where(b => b.IsDeleted == false).ToList();

                    return new ApiRespone
                    {
                        Succeeded = true,
                        Message = activeBooks.Any() ? "Books retrieved successfully." : "No books available.",
                        Data = activeBooks
                    };
                }
                catch (Exception ex)
                {
                    return new ApiRespone { Succeeded = false, Message = $"Error: {ex.Message}", Data = null };
                }
            }

            // ✅ Lấy sách theo ID
            public async Task<ApiRespone> GetBookByIdAsync(long id)
            {
                try
                {
                    var book = await _baseRepository.GetByIdAsync(id);
                    if (book == null || book.IsDeleted == true)
                        return new ApiRespone { Succeeded = false, Message = "Book not found.", Data = null };

                    return new ApiRespone
                    {
                        Succeeded = true,
                        Message = "Book retrieved successfully.",
                        Data = book
                    };
                }
                catch (Exception ex)
                {
                    return new ApiRespone { Succeeded = false, Message = $"Error: {ex.Message}", Data = null };
                }
            }
        }
    }

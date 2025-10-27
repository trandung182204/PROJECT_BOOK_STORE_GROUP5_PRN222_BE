using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly BookStoreContext _bookStoreContext;
        public CategoryRepository(BookStoreContext context) : base(context)
        {
            _bookStoreContext = context;
        }

        public async Task<Category?> GetCategoryByUserIdAsync(long userId)
        {
            return await _bookStoreContext.Categories.Include(c => c.BookCategories)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }



    }
}

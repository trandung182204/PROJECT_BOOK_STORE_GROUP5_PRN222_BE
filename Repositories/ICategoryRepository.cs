using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByUserIdAsync(long userId);

    }
}

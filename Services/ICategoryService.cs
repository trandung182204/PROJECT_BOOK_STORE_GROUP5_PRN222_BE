using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoryAsync();
        Task<Category?> GetCategoryByIdAsync(long id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(long id);
        Task<bool> ExistsAsync(string code, string name);

    }
}

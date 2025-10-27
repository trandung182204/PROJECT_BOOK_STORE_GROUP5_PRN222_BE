using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface ICategoryService
    {
        Task<ApiRespone> GetAllCategoryAsync();
        Task<ApiRespone> GetCategoryByIdAsync(long id);
        Task<ApiRespone> AddCategoryAsync(CategoryDTO category);
        Task<ApiRespone> UpdateCategoryAsync(long id,CategoryDTO category);
        Task<ApiRespone> DeleteCategoryAsync(long id);
        Task<ApiRespone> ExistsAsync(string code, string name, long? excludeId = null);

    }
}

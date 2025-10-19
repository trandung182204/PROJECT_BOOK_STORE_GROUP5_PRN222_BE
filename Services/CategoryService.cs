using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBaseRepository<Category> _baseRepository;
        private readonly BookStoreContext _context;

        // ✅ Constructor để inject dependency
        public CategoryService(
            ICategoryRepository categoryRepository,
            IBaseRepository<Category> baseRepository,
            BookStoreContext context)
        {
            _categoryRepository = categoryRepository;
            _baseRepository = baseRepository;
            _context = context;
        }

        // ✅ Lấy tất cả danh mục
        public async Task<IEnumerable<Category>> GetAllCategoryAsync()
        {
            var categories = await _baseRepository.GetAllAsync();
            if (categories == null || !categories.Any())
                throw new InvalidOperationException("No categories available.");

            return categories.Where(c => !c.IsDeleted);
        }

        // ✅ Lấy danh mục theo ID
        public async Task<Category?> GetCategoryByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid category ID.");

            var category = await _baseRepository.GetByIdAsync(id);
            if (category == null || category.IsDeleted)
                throw new KeyNotFoundException($"Category with ID {id} not found or has been deleted.");

            return category;
        }

        // ✅ Thêm danh mục mới
        public async Task AddCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            if (string.IsNullOrWhiteSpace(category.CategoryCode))
                throw new ArgumentException("Category code cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name cannot be null or empty.");
            if (category.ParentId.HasValue && category.ParentId <= 0)
                throw new ArgumentException("Parent Id must be greater than 0.");

            category.IsDeleted = false;
            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = null;

            await _baseRepository.AddAsync(category);
        }

        // ✅ Cập nhật danh mục
        public async Task UpdateCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            if (string.IsNullOrWhiteSpace(category.CategoryCode))
                throw new ArgumentException("Category code cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name cannot be null or empty.");

            var existingCategory = await _baseRepository.GetByIdAsync(category.Id);
            if (existingCategory == null)
                throw new KeyNotFoundException("Category not found.");

            if (existingCategory.IsDeleted)
                throw new InvalidOperationException("Cannot update a deleted category.");

            existingCategory.CategoryCode = category.CategoryCode;
            existingCategory.CategoryName = category.CategoryName;
            existingCategory.ParentId = category.ParentId;
            existingCategory.UpdatedAt = DateTime.Now;

            await _baseRepository.UpdateAsync(existingCategory);
        }

        // ✅ Xóa mềm danh mục
        public async Task DeleteCategoryAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid category ID.");

            var category = await _baseRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            if (category.IsDeleted)
                throw new InvalidOperationException("Category is already deleted.");

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.Now;

            await _baseRepository.UpdateAsync(category);
        }

        public async Task<bool> ExistsAsync(string code, string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.CategoryCode == code || c.CategoryName == name);
        }

    }
}

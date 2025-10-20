using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category> _baseRepository;

        public CategoryService(IBaseRepository<Category> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        // ✅ Lấy tất cả danh mục
        public async Task<ApiRespone> GetAllCategoryAsync()
        {
            try
            {
                var categories = await _baseRepository.GetAllAsync();
                var active = categories.Where(c => !c.IsDeleted).ToList();

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Get all categories successfully.",
                    Data = active
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone
                {
                    Succeeded = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        // ✅ Lấy danh mục theo ID
        public async Task<ApiRespone> GetCategoryByIdAsync(long id)
        {
            try
            {
                var category = await _baseRepository.GetByIdAsync(id);
                if (category == null || category.IsDeleted)
                {
                    return new ApiRespone
                    {
                        Succeeded = false,
                        Message = "Category not found.",
                        Data = null
                    };
                }

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Category retrieved successfully.",
                    Data = category
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone
                {
                    Succeeded = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        // ✅ Thêm danh mục
        public async Task<ApiRespone> AddCategoryAsync(CategoryDTO category)
        {
            try
            {
                if (category == null)
                    return new ApiRespone { Succeeded = false, Message = "Category cannot be null." };

                if (string.IsNullOrWhiteSpace(category.CategoryCode))
                    return new ApiRespone { Succeeded = false, Message = "CategoryCode is required." };

                if (string.IsNullOrWhiteSpace(category.CategoryName))
                    return new ApiRespone { Succeeded = false, Message = "CategoryName is required." };

                var exists = await ExistsAsync(category.CategoryCode, category.CategoryName);
                if ((bool)exists.Data)
                    return new ApiRespone { Succeeded = false, Message = "CategoryCode or CategoryName already exists." };

                var newCategory = new Category
                {
                    CategoryCode = category.CategoryCode,
                    CategoryName = category.CategoryName,
                    IsDeleted = false
                };

                await _baseRepository.AddAsync(newCategory);

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Category created successfully.",
                    Data = newCategory
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone { Succeeded = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }

        // ✅ Cập nhật danh mục
        public async Task<ApiRespone> UpdateCategoryAsync(long id, CategoryDTO category)
        {
            if (category == null)
                return new ApiRespone { Succeeded = false, Message = "Category cannot be null." };

            if (string.IsNullOrWhiteSpace(category.CategoryCode))
                return new ApiRespone { Succeeded = false, Message = "Category code cannot be empty." };

            if (string.IsNullOrWhiteSpace(category.CategoryName))
                return new ApiRespone { Succeeded = false, Message = "Category name cannot be empty." };

            try
            {
                var existing = await _baseRepository.GetByIdAsync(id);
                if (existing == null || existing.IsDeleted)
                    return new ApiRespone { Succeeded = false, Message = "Category not found." };

                var exists = await ExistsAsync(category.CategoryCode, category.CategoryName, id);
                if (exists.Succeeded && (bool)exists.Data)
                    return new ApiRespone { Succeeded = false, Message = "CategoryCode or CategoryName already exists." };

                existing.CategoryCode = category.CategoryCode.Trim();
                existing.CategoryName = category.CategoryName.Trim();

                await _baseRepository.UpdateAsync(existing);

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Category updated successfully.",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone
                {
                    Succeeded = false,
                    Message = $"Error updating category: {ex.Message}",
                    Data = null
                };
            }
        }



        // ✅ Xóa mềm danh mục
        public async Task<ApiRespone> DeleteCategoryAsync(long id)
        {
            try
            {
                var category = await _baseRepository.GetByIdAsync(id);
                if (category == null)
                    return new ApiRespone { Succeeded = false, Message = "Category not found." };
                if (category.IsDeleted)
                    return new ApiRespone { Succeeded = false, Message = "Category was deleted." };

                category.IsDeleted = true;
                await _baseRepository.UpdateAsync(category);

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Category deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone { Succeeded = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }

        // ✅ Kiểm tra trùng code hoặc name
        public async Task<ApiRespone> ExistsAsync(string categoryCode, string categoryName, long? excludeId = null)
        {
            try
            {
                var categories = await _baseRepository.GetAllAsync();
                bool exists = categories.Any(c =>
                    !c.IsDeleted &&
                    (c.CategoryCode.Equals(categoryCode, StringComparison.OrdinalIgnoreCase) ||
                     c.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase)) &&
                    (!excludeId.HasValue || c.Id != excludeId.Value));

                return new ApiRespone
                {
                    Succeeded = true,
                    Message = "Check completed.",
                    Data = exists
                };
            }
            catch (Exception ex)
            {
                return new ApiRespone { Succeeded = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }
    }
}

using PROJECT_BOOK_STORE_GROUP5_PRN222.Data;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IUserService
    {
        Task<ApiResponse> GetAllUsersAsync();
        Task<ApiResponse> GetUserByIdAsync(string id);
        Task<ApiResponse> UpdateUserAsync(string id, UpdateUserModel model);
        Task<ApiResponse> SoftDeleteUserAsync(string id);
        Task<ApiResponse> ToggleUserStatusAsync(string id);
    }
}
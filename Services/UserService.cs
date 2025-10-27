using Microsoft.AspNetCore.Identity;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Data;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class UserService : IUserService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IAccountRepository accountRepository, UserManager<ApplicationUser> userManager)
        {
            _accountRepository = accountRepository;
            _userManager = userManager;
        }

        public async Task<ApiResponse> GetAllUsersAsync()
        {
            var users = await _accountRepository.GetAllAsync();
            return new ApiResponse
            {
                Succeeded = true,
                Message = "lay tât ca user",
                Data = users
            };
        }

        public async Task<ApiResponse> GetUserByIdAsync(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            if (user == null)
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = "User Not Found"
                };
            return new ApiResponse
            {
                Succeeded = true,
                Message = "User Found",
                Data = user
            };
        }

        public async Task<ApiResponse> UpdateUserAsync(string id, UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return new ApiResponse
            {
                Succeeded = false,
                Message = "User not found!"
            };

            user.Email = model.Email ?? user.Email;
            user.UserName = model.UserName ?? user.UserName;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

            await _userManager.UpdateAsync(user);
            return new ApiResponse
            {
                Succeeded = true,
                Message = "User successfully updated!",
                Data = user
            };
        }

        public async Task<ApiResponse> SoftDeleteUserAsync(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            if (user == null) return new ApiResponse
            {
                Succeeded = false,
                Message = "User not found!"
            };

            user.IsDeleted = true;
            await _accountRepository.UpdateAsync(user);
            return new ApiResponse
            {
                Succeeded = true,
                Message = "User successfully deleted!",
            };
        }

        public async Task<ApiResponse> ToggleUserStatusAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return new ApiResponse
            {
                Succeeded = false,
                Message = "User not found!"
            };

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
                return new ApiResponse
                {
                    Succeeded = true,
                    Message = "User unlocked!",
                };
            }
            else
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                await _userManager.UpdateAsync(user);
                return new ApiResponse
                {
                    Succeeded = true,
                    Message = "User locked!",
                };
            }
        }
    }
}

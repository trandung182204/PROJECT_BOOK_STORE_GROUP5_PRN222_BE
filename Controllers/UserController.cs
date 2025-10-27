using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;
using System.Security.Claims;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        // [Admin] Get all users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await userService.GetAllUsersAsync());
        }

        // [User or Admin] Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentRole != "Admin" && currentUserId != id)
            {
                return Ok(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Invalid role"
                });
            }
            return Ok(await userService.GetUserByIdAsync(id));
        }

        // [User or Admin] Update user info
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserModel model)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentRole != "Admin" && currentUserId != id)
            {
                return Ok(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Invalid role"
                });
            }
            return Ok(await userService.UpdateUserAsync(id, model));
        }

        // [Admin only] Soft delete user
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            return Ok(await userService.SoftDeleteUserAsync(id));
        }

        // [Admin only] Lock / Unlock user
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            return Ok(await userService.ToggleUserStatusAsync(id));
        }
    }
}

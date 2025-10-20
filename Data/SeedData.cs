using Microsoft.AspNetCore.Identity;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var seedConfig = configuration.GetSection("SeedAccounts");
            var roles = seedConfig.GetSection("Roles").Get<string[]>() ?? [];
            var users = seedConfig.GetSection("Users").Get<List<SeedUserModel>>() ?? [];

            // 🧹 Xóa toàn bộ users và roles cũ (tuỳ chọn)
            var allUsers = userManager.Users.ToList();
            foreach (var u in allUsers)
            {
                await userManager.DeleteAsync(u);
            }

            var allRoles = roleManager.Roles.ToList();
            foreach (var r in allRoles)
            {
                await roleManager.DeleteAsync(r);
            }

            // 🧱 Tạo roles mới
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 👤 Tạo users mới
            foreach (var u in users)
            {
                var user = new ApplicationUser
                {
                    UserName = u.Email,
                    Email = u.Email,
                    FullName = u.Email.Split('@')[0],
                    EmailConfirmed = true,
                    Address = "N/A", // 👈 thêm dòng này
                    AvatarUrl = "", // (nếu cột này cũng NOT NULL)
                    CreatedAt = DateTime.Now
                };


                var result = await userManager.CreateAsync(user, u.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, u.Role);
                }
                else
                {
                    Console.WriteLine($"❌ Failed to create user {u.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            Console.WriteLine("✅ Seed data (users & roles) recreated successfully!");
        }
    }
}

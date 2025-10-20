using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<BookStoreContext>();

            var seedConfig = configuration.GetSection("SeedAccounts");
            var roles = seedConfig.GetSection("Roles").Get<string[]>() ?? [];
            var users = seedConfig.GetSection("Users").Get<List<SeedUserModel>>() ?? [];

            // 🧱 1️⃣ Tạo roles nếu chưa có
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"✅ Created role: {role}");
                }
            }

            // 👤 2️⃣ Tạo users nếu chưa có
            foreach (var u in users)
            {
                var existingUser = await userManager.FindByEmailAsync(u.Email);
                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FullName = u.Email.Split('@')[0],
                        EmailConfirmed = true,
                        Address = "N/A",
                        AvatarUrl = "",
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                    };

                    var result = await userManager.CreateAsync(user, u.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, u.Role);
                        Console.WriteLine($"✅ Created user: {u.Email} ({u.Role})");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Failed to create user {u.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    // Nếu user đã có, đảm bảo họ nằm trong đúng role
                    var rolesOfUser = await userManager.GetRolesAsync(existingUser);
                    if (!rolesOfUser.Contains(u.Role))
                    {
                        await userManager.AddToRoleAsync(existingUser, u.Role);
                        Console.WriteLine($"🔄 Added missing role '{u.Role}' to {u.Email}");
                    }
                }
            }

            Console.WriteLine("✅ Seed data completed successfully (idempotent version).");
        }
    }
}

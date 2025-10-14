using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Data;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Helpers;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IConfiguration configuration;
    private readonly RoleManager<IdentityRole> roleManager;

    public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration, RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
        this.roleManager = roleManager;
    }

    public async Task<IdentityResult> SignUpAsync(SignUp signUp)
    {
        var user = new ApplicationUser
        {
            FullName = signUp.FullName,
            Address = signUp.Address,
            AvatarUrl = signUp.AvatarUrl,
            Email = signUp.Email,
            UserName = signUp.Email,
        };
        var result = await userManager.CreateAsync(user, signUp.Password);
        if (result.Succeeded)
        {
            // kiểm tra role Customer đã có chưa
            if (!await roleManager.RoleExistsAsync(AppRole.Customer))
            {
                await roleManager.CreateAsync(new IdentityRole(AppRole.Customer));
            }
            
            await userManager.AddToRoleAsync(user, AppRole.Customer);
        }
        return result;
    }

    public async Task<string> SignInAsync(SignIn signIn)
    {
        var user = await userManager.FindByEmailAsync(signIn.Email);
        var passwordValid = await userManager.CheckPasswordAsync(user, signIn.Password);
        if (user == null || !passwordValid)
        {
            return string.Empty;
        }
        var result = await signInManager.PasswordSignInAsync(signIn.Email, signIn.Password, false, false);
        if (!result.Succeeded)
        {
            return string.Empty;
        }

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, signIn.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var userRoles = await userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }
        var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
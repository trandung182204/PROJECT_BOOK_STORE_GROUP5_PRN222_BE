using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Data;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Helpers;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IConfiguration configuration;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IRefreshTokenRepository refreshTokenRepository;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration, RoleManager<IdentityRole> roleManager,
        IRefreshTokenRepository refreshTokenRepository)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
        this.roleManager = roleManager;
        this.refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ApiResponse> SignUpAsync(SignUp signUp)
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

        return new ApiResponse
        {
            Succeeded = true,
            Message = "Sign up successfully.",
            Data = result
        };
    }

    public async Task<ApiResponse> SignInAsync(SignIn signIn)
    {
        var user = await userManager.FindByEmailAsync(signIn.Email);
        if (user == null)
            return new ApiResponse
            {
                Succeeded = false,
                Message = "User not found.",
            };
        var passwordValid = user != null && await userManager.CheckPasswordAsync(user, signIn.Password);
        if (!passwordValid)
        {
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Invalid password.",
            };
        }

        var result = await signInManager.PasswordSignInAsync(signIn.Email, signIn.Password, false, false);
        if (!result.Succeeded)
        {
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Invalid password.",
            };
        }

        return new ApiResponse
        {
            Succeeded = true,
            Message = "Generated Token.",
            Data = await GenerateTokensAsync(user)
        };
    }

    private async Task<TokenModel> GenerateTokensAsync(ApplicationUser user)
    {
        var userRoles = await userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddMinutes(30),
            claims: authClaims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        var refreshEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            JwtId = token.Id,
            Token = refreshToken,
            IsUsed = false,
            IsRevoked = false,
            IssuedAt = DateTime.Now,
            ExpiredAt = DateTime.Now.AddHours(1),
            UserId = user.Id
        };

        await refreshTokenRepository.AddAsync(refreshEntity);
        await refreshTokenRepository.SaveChangesAsync();

        return new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64]; // độ dài 64 bytes = 512 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber); // chuyển thành chuỗi base64
    }

    public async Task<ApiResponse> RenewTokenAsync(TokenModel tokenModel)
    {
        var jwtHandler = new JwtSecurityTokenHandler();

        JwtSecurityToken jwtToken;
        ClaimsPrincipal principal;

        try
        {
            principal = jwtHandler.ValidateToken(tokenModel.AccessToken, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                SignatureValidator = (token, _) => new JwtSecurityToken(token),
                ValidateLifetime = false
            }, out var validatedToken);

            jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return new ApiResponse
                {
                    Succeeded = false,
                    Message = "Invalid token signature algorithm",
                };
            }
        }
        catch
        {
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Invalid access token format",
            };
        }

        // ✅ Check access token expired
        var expClaim = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value;
        if (expClaim == null || !long.TryParse(expClaim, out var utcExpireDate))
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Access token missing expiration claim",
            };
        var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
        if (expireDate > DateTime.UtcNow)
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Access token has not yet expired",
            };
        // ✅ Check refresh token in DB
        var storedToken = await refreshTokenRepository.GetByTokenAsync(tokenModel.RefreshToken);


        if (storedToken == null)
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Refresh token does not exist",
            };
        if (storedToken.IsUsed)
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Refresh token has been used",
            };
        if (storedToken.IsRevoked)
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Refresh token has been revoked",
            };
        if (storedToken.ExpiredAt < DateTime.UtcNow)
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Refresh token has expired",
            };
        var jti = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
        if (storedToken.JwtId != jti)
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Token doesn't match",
            };
        // ✅ Mark old token as used
        storedToken.IsUsed = true;
        storedToken.IsRevoked = true;
        await refreshTokenRepository.UpdateAsync(storedToken);

        // ✅ Generate new tokens
        var email = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        var user = await userManager.FindByEmailAsync(email);
        var newTokens = await GenerateTokensAsync(user);

        await refreshTokenRepository.SaveChangesAsync();

        return new ApiResponse
        {
            Succeeded = true,
            Message = "Token refreshed successfully",
            Data = newTokens
        };
    }

    private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(utcExpireDate);
        return dateTimeOffset.UtcDateTime;
    }

    public async Task<ApiResponse> LogoutAsync(string refreshToken)
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken == null)
        {
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Refresh token not found"
            };
        }

        if (storedToken.IsRevoked || storedToken.IsUsed)
        {
            return new ApiResponse
            {
                Succeeded = false,
                Message = "Refresh token already invalidated"
            };
        }

        storedToken.IsRevoked = true;
        storedToken.IsUsed = true;
        await refreshTokenRepository.UpdateAsync(storedToken);
        await refreshTokenRepository.SaveChangesAsync();

        return new ApiResponse
        {
            Succeeded = true,
            Message = "Logged out successfully"
        };
    }
}
using Microsoft.AspNetCore.Identity;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

public interface IAccountService
{
    public Task<ApiResponse> SignUpAsync(SignUp signUp);
    public Task<ApiResponse> SignInAsync(SignIn signIn);

    public Task<ApiResponse> RenewTokenAsync(TokenModel tokenModel);
    
    Task<ApiResponse> LogoutAsync(string refreshToken);

}
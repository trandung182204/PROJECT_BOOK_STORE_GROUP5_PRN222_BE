using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

public interface IAccountRepository
{
    public Task<IdentityResult> SignUpAsync(SignUp signUp);
    public Task<string> SignInAsync(SignIn signIn);
    
    
}
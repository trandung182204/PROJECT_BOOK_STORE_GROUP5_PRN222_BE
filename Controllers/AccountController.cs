
using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccountService service) : ControllerBase
    {
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUp signUp)
        {
            return Ok(await service.SignUpAsync(signUp));
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignIn signIn)
        {
            return Ok(await service.SignInAsync(signIn));
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel tokenModel)
        {
            return Ok(await service.RenewTokenAsync(tokenModel));
        }
        
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(LogoutRequest refreshToken)
        {
            return Ok(await service.LogoutAsync(refreshToken.RefreshToken));
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using ShopListApp.Commands;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Services;

namespace ShopListApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserCommand cmd)
        {
            (string accessToken, string refreshToken) = await _authService.RegisterUser(cmd);
            return Ok(new { accessToken, refreshToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand cmd)
        {
            (string accessToken, string refreshToken) = await _authService.LoginUser(cmd);
            return Ok(new { accessToken, refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand cmd)
        {
            string accessToken = await _authService.RefreshAccessToken(cmd);
            return Ok(new { accessToken });
        }
    }
}

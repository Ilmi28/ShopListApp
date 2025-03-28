using Microsoft.AspNetCore.Mvc;
using ShopListApp.Commands;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Interfaces.IServices;

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
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand cmd)
        {
            var response = await _authService.RegisterUser(cmd);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand cmd)
        {
            var response = await _authService.LoginUser(cmd);
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand cmd)
        {
            string identityToken = await _authService.RefreshAccessToken(cmd);
            return Ok(new { identityToken });
        }
    }
}

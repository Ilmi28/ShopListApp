using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Interfaces.IServices;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand cmd)
    {
        var response = await authService.RegisterUser(cmd);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand cmd)
    {
        var response = await authService.LoginUser(cmd);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand cmd)
    {
        string identityToken = await authService.RefreshAccessToken(cmd);
        return Ok(new { identityToken });
    }
}

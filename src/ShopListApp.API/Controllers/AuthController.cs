using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Interfaces.IServices;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ITokenManager tokenManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand cmd)
    {
        var response = await authService.RegisterUser(cmd);
        
        Response.Cookies.Append("accessToken", response.IdentityToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None, 
            Expires = DateTime.UtcNow.AddMinutes(tokenManager.GetAccessTokenExpirationMinutes())
        });

        Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(tokenManager.GetRefreshTokenExpirationDays())
        });

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand cmd)
    {
        var response = await authService.LoginUser(cmd);
        
        Response.Cookies.Append("accessToken", response.IdentityToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(tokenManager.GetAccessTokenExpirationMinutes())
        });

        Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(tokenManager.GetRefreshTokenExpirationDays())
        });

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        string newAccessToken = await authService.RefreshAccessToken(new RefreshTokenCommand { RefreshToken = refreshToken });
        Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(tokenManager.GetAccessTokenExpirationMinutes())
        });

        return Ok();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
        return Ok();
    }
    
}

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
    [ProducesResponseType(StatusCodes.Status201Created)]
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

        return Created();
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

        return NoContent();
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

        return NoContent();
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };
        
        Response.Cookies.Delete("accessToken", cookieOptions);
        Response.Cookies.Delete("refreshToken", cookieOptions);
        return NoContent();
    }
    
}

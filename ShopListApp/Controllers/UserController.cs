using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopListApp.Interfaces;
using ShopListApp.Managers;
using ShopListApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopListApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private ITokenManager _tokenManager;
        public UserController(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }
        [HttpGet]
        public IActionResult GetToken()
        {
            var user = new User
            {
                Id = "1",
                UserName = "test",
                Email = "ilmialiev28@gmail.com"
            };
            return Ok(_tokenManager.GenerateIdentityToken(user));
        }

        [HttpGet("hello")]
        [Authorize]
        public IActionResult Hello()
        {
            return Ok("Hello, World!");
        }
    }
}

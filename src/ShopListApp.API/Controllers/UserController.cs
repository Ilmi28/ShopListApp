using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopListApp.Commands;
using ShopListApp.Core.Commands.Delete;
using ShopListApp.Interfaces;
using ShopListApp.Managers;
using ShopListApp.Models;
using ShopListApp.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace ShopListApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserCommand cmd)
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _userService.UpdateUser(id, cmd);
            return Ok(id);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromBody]DeleteUserCommand cmd)
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _userService.DeleteUser(id, cmd);
            return Ok(id);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetUser()
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userView = await _userService.GetUserById(id);
            return Ok(userView);
        }
    }
}

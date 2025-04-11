using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Commands.Delete;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Interfaces.IServices;
using System.Security.Claims;

namespace ShopListApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody]UpdateUserCommand cmd)
    {
        string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await userService.UpdateUser(id, cmd);
        return Ok(id);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromBody]DeleteUserCommand cmd)
    {
        string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await userService.DeleteUser(id, cmd);
        return Ok(id);
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetUser()
    {
        string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userView = await userService.GetUserById(id);
        return Ok(userView);
    }
}

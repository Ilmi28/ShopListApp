using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Commands.Delete;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Interfaces.IServices;
using System.Security.Claims;
using ShopListApp.Core.Responses;

namespace ShopListApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPut("update")]
    [ProducesResponseType( StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUser([FromBody]UpdateUserCommand cmd)
    {
        string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await userService.UpdateUser(id, cmd);
        return NoContent();
    }

    [HttpPost("delete")]
    [ProducesResponseType( StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUser([FromBody]DeleteUserCommand cmd)
    {
        string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await userService.DeleteUser(id, cmd);
        return NoContent();
    }

    [HttpGet("get")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser()
    {
        string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userView = await userService.GetUserById(id);
        return Ok(userView);
    }
}

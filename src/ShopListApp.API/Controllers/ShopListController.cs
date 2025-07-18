using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopListApp.API.Filters;
using ShopListApp.Core.Commands.Create;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Interfaces.IServices;
using System.Security.Claims;
using ShopListApp.Core.Responses;

namespace ShopListApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/shoplist")]
public class ShopListController(IShopListService shopListService, IAuthorizationService authorizationService) : ControllerBase
{
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateShopList([FromBody] CreateShopListCommand cmd)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await shopListService.CreateShopList(userId, cmd);
        return Created($"api/shoplist/{cmd.Name}", null);
    }

    [HttpDelete("delete/{shopListId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteShopList(int shopListId)
    {
        var shopList= await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await shopListService.DeleteShopList(shopListId);
        return NoContent();
    }

    [HttpPatch("update/add-product/{shopListId}/{productId}")]
    [QuantityFilter]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddProductToShopList(int shopListId, int productId, [FromQuery] int quantity = 1)
    {
        var shopList = await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await shopListService.AddProductToShopList(shopListId, productId, quantity);
        return NoContent();
    }

    [HttpPatch("update/delete-product/{shopListId}/{productId}")]
    [QuantityFilter]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveProductFromShopList(int shopListId, int productId, [FromQuery] int quantity = int.MaxValue)
    {
        var shopList = await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await shopListService.RemoveProductFromShopList(shopListId, productId, quantity);
        return NoContent();
    }

    [HttpGet("get/{shopListId}")]
    [ProducesResponseType(typeof(ICollection<ShopListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShopList(int shopListId)
    {
        var shopList = await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        return Ok(shopList);
    }

    [HttpGet("get-all")]
    [ProducesResponseType(typeof(ICollection<ShopListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShopLists()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var shopLists = await shopListService.GetShopListsForUser(userId);
        return Ok(shopLists);
    }

    [HttpPut("update/{shopListId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateShopList(int shopListId, [FromBody] UpdateShopListCommand cmd)
    {
        var shopList = await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await shopListService.UpdateShopList(shopListId, cmd);
        return NoContent();
    }

}

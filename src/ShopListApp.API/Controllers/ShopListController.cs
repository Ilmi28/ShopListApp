using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopListApp.API.Filters;
using ShopListApp.Core.Commands.Create;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Interfaces.IServices;
using System.Security.Claims;

namespace ShopListApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/shoplist")]
public class ShopListController(IShopListService shopListService, IAuthorizationService authorizationService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateShopList([FromBody] CreateShopListCommand cmd)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await shopListService.CreateShopList(userId, cmd);
        return Ok();
    }

    [HttpDelete("delete/{shopListId}")]
    public async Task<IActionResult> DeleteShopList(int shopListId)
    {
        var shopList= await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await shopListService.DeleteShopList(shopListId);
        return Ok();
    }

    [HttpPatch("update/add-product/{shopListId}/{productId}")]
    [QuantityFilter]
    public async Task<IActionResult> AddProductToShopList(int shopListId, int productId, [FromQuery] int quantity = 1)
    {
        var shopList = await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await shopListService.AddProductToShopList(shopListId, productId, quantity);
        return Ok();
    }

    [HttpPatch("update/delete-product/{shopListId}/{productId}")]
    [QuantityFilter]
    public async Task<IActionResult> RemoveProductFromShopList(int shopListId, int productId, [FromQuery] int quantity = int.MaxValue)
    {
        var shopList = await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await shopListService.RemoveProductFromShopList(shopListId, productId, quantity);
        return Ok();
    }

    [HttpGet("get/{shopListId}")]
    public async Task<IActionResult> GetShopList(int shopListId)
    {
        var shopList = await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        return Ok(shopList);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllShopLists()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var shopLists = await shopListService.GetShopListsForUser(userId);
        return Ok(shopLists);
    }

    [HttpPut("update/{shopListId}")]
    public async Task<IActionResult> UpdateShopList(int shopListId, [FromBody] UpdateShopListCommand cmd)
    {
        var shopList = await shopListService.GetShopListById(shopListId);
        var result = await authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await shopListService.UpdateShopList(shopListId, cmd);
        return Ok();
    }

}

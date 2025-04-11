using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Interfaces.IServices;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/store")]
public class StoreController(IStoreService storeService) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetStores()
    {
        return Ok(await storeService.GetStores());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStore(int id)
    {
        return Ok(await storeService.GetStoreById(id));
    }
}

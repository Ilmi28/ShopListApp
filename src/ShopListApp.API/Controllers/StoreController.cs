using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Interfaces.IServices;
using ShopListApp.Core.Responses;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/store")]
public class StoreController(IStoreService storeService) : ControllerBase
{
    [HttpGet("get-all")]
    [ProducesResponseType(typeof(ICollection<StoreResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStores()
    {
        return Ok(await storeService.GetStores());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStore(int id)
    {
        return Ok(await storeService.GetStoreById(id));
    }
}

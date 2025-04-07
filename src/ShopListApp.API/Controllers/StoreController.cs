using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Interfaces.IServices;

namespace ShopListApp.API.Controllers
{
    [ApiController]
    [Route("api/store")]
    public class StoreController : ControllerBase
    {
        private IStoreService _storeService;
        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetStores()
        {
            return Ok(await _storeService.GetStores());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStore(int id)
        {
            return Ok(await _storeService.GetStoreById(id));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ShopListApp.Commands;
using ShopListApp.Interfaces.IServices;

namespace ShopListApp.Controllers
{
    [ApiController]
    [Route("api/shoplist")]
    public class ShopListController : ControllerBase
    {
        private readonly IShopListService _shopListService;
        public ShopListController(IShopListService shopListService)
        {
            _shopListService = shopListService;
        }

        [HttpPost("create")]
        public IActionResult CreateShopList([FromBody] CreateShopListCommand cmd)
        {
            _shopListService.CreateShopList(cmd);
            return Ok();
        }

        [HttpDelete("delete/{shopListId}")]
        public IActionResult DeleteShopList(int shopListId)
        {
            _shopListService.DeleteShopList(shopListId);
            return Ok();
        }

        [HttpPatch("update/delete-product/{shopListId}/{productId}")]
        public IActionResult AddProductToShopList(int shopListId, int productId)
        {
            _shopListService.AddProductToShopList(shopListId, productId);
            return Ok();
        }

        [HttpPatch("update/add-product/{shopListId}/{productId}")]
        public IActionResult RemoveProductFromShopList(int shopListId, int productId)
        {
            _shopListService.RemoveProductFromShopList(shopListId, productId);
            return Ok();
        }

        [HttpGet("get/{shopListId}")]
        public IActionResult GetShopListProducts(int shopListId)
        {
            var products = _shopListService.GetShopListProducts(shopListId);
            return Ok(products);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Interfaces.IServices;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllProducts()
    {
        return Ok(await productService.GetAllProducts());
    }

    [HttpGet("get-by-category/{id}")]
    public async Task<IActionResult> GetProductsByCategory(int id)
    {
        return Ok(await productService.GetProductsByCategoryId(id));
    }


    [HttpGet("get-by-store/{id}")]
    public async Task<IActionResult> GetProductsByStore(int id)
    {
        return Ok(await productService.GetProductsByStoreId(id));
    }

    [HttpPatch("refresh")]
    public async Task<IActionResult> RefreshProducts()
    {
        await productService.RefreshProducts();
        return Ok();
    }

    [HttpGet("get-categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await productService.GetCategories());
    }

    [HttpGet("get-product/{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        return Ok(await productService.GetProductById(id));
    }
}

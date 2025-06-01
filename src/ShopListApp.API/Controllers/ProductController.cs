using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Interfaces.IServices;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllProducts(int pageNumber, int pageSize)
    {
        return Ok(await productService.GetPagedAllProducts(pageNumber, pageSize));
    }

    [HttpGet("get-by-category/{id}")]
    public async Task<IActionResult> GetProductsByCategory(int id,  int pageNumber, int pageSize)
    {
        var response = await productService.GetPagedProductsByCategoryId(id, pageNumber, pageSize);
        return Ok(response);
    }


    [HttpGet("get-by-store/{id}")]
    public async Task<IActionResult> GetProductsByStore(int id, int pageNumber, int pageSize)
    {
        return Ok(await productService.GetPagedProductsByStoreId(id,  pageNumber, pageSize));
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

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(string q, int pageNumber, int pageSize)
    {
        return Ok(await productService.SearchProducts(q, pageNumber, pageSize));   
    }
}

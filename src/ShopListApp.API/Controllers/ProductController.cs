using Microsoft.AspNetCore.Mvc;
using ShopListApp.Core.Interfaces.IServices;
using ShopListApp.Core.Responses;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet("get-all")]
    [ProducesResponseType(typeof(PagedProductResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProducts(int pageNumber, int pageSize)
    {
        return Ok(await productService.GetPagedAllProducts(pageNumber, pageSize));
    }

    [HttpGet("get-by-category/{id}")]
    [ProducesResponseType(typeof(PagedProductResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByCategory(int id,  int pageNumber, int pageSize)
    {
        var response = await productService.GetPagedProductsByCategoryId(id, pageNumber, pageSize);
        return Ok(response);
    }


    [HttpGet("get-by-store/{id}")]
    [ProducesResponseType(typeof(PagedProductResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByStore(int id, int pageNumber, int pageSize)
    {
        return Ok(await productService.GetPagedProductsByStoreId(id,  pageNumber, pageSize));
    }

    [HttpPatch("refresh")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RefreshProducts()
    {
        await productService.RefreshProducts();
        return NoContent();
    }

    [HttpGet("get-categories")]
    [ProducesResponseType(typeof(ICollection<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await productService.GetCategories());
    }

    [HttpGet("get-product/{id}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductById(int id)
    {
        return Ok(await productService.GetProductById(id));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ICollection<PagedProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchProducts(string q, int pageNumber, int pageSize)
    {
        return Ok(await productService.SearchProducts(q, pageNumber, pageSize));   
    }
}

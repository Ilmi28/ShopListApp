using ShopListApp.Core.Responses;

namespace ShopListApp.Core.Interfaces.IServices;

public interface IProductService
{
    Task RefreshProducts();
    Task<ICollection<ProductResponse>> GetProductsByCategoryId(int categoryId);
    Task<PagedProductResponse> GetPagedProductsByCategoryId(int categoryId, int pageNumber, int pageSize);
    Task<ICollection<ProductResponse>> GetProductsByStoreId(int storeId);
    Task<PagedProductResponse> GetPagedProductsByStoreId(int storeId, int pageNumber, int pageSize);
    Task<ICollection<ProductResponse>> GetAllProducts();
    Task<PagedProductResponse> GetPagedAllProducts(int pageNumber, int pageSize);
    Task<ICollection<CategoryResponse>> GetCategories();
    Task<ProductResponse?> GetProductById(int id);
    Task<PagedProductResponse> SearchProducts(string search, int pageNumber, int pageSize);
}

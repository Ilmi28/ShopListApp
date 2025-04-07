using ShopListApp.Core.Responses;

namespace ShopListApp.Core.Interfaces.IServices
{
    public interface IProductService
    {
        Task RefreshProducts();
        Task<ICollection<ProductResponse>> GetProductsByCategoryId(int categoryId);
        Task<ICollection<ProductResponse>> GetProductsByStoreId(int storeId);
        Task<ICollection<ProductResponse>> GetAllProducts();
        Task<ICollection<CategoryResponse>> GetCategories();
        Task<ProductResponse?> GetProductById(int id);
    }
}

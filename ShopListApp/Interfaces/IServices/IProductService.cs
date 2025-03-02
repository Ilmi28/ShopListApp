using ShopListApp.ViewModels;

namespace ShopListApp.Interfaces.IServices
{
    public interface IProductService
    {
        Task RefreshProducts();
        Task<ICollection<ProductView>> GetProductsByCategoryId(int categoryId);
        Task<ICollection<ProductView>> GetProductsByStoreId(int storeId);
        Task<ICollection<ProductView>> GetAllProducts();
        Task<ICollection<CategoryView>> GetCategories();
        Task<ProductView?> GetProductById(int id);
    }
}

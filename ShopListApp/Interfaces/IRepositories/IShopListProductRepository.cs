using ShopListApp.Models;

namespace ShopListApp.Interfaces.IRepositories
{
    public interface IShopListProductRepository
    {
        Task AddShopListProduct(ShopListProduct shopListProduct);
        Task<bool> RemoveShopListProduct(int shopListId, int productId);
        Task<ICollection<Product>> GetProductsForShopList(int shopListId);

    }
}

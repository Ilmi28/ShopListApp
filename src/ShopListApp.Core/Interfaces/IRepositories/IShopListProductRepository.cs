using ShopListApp.Core.Models;

namespace ShopListApp.Core.Interfaces.IRepositories;

public interface IShopListProductRepository
{
    Task AddShopListProduct(ShopListProduct shopListProduct);
    Task<bool> RemoveShopListProduct(int shopListId, int productId);
    Task<ICollection<Product>> GetProductsForShopList(int shopListId);
    Task<ShopListProduct?> GetShopListProduct(int shopListId, int productId);
    Task<bool> UpdateShopListProductQuantity(int id, int newQuantity);
}

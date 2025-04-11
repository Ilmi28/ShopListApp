using ShopListApp.Core.Models;

namespace ShopListApp.Core.Interfaces.IRepositories;

public interface IShopListRepository
{
    Task AddShopList(ShopList shopList);
    Task<bool> RemoveShopList(int id);
    Task<bool> UpdateShopList(int id, ShopList updatedShopList);
    Task<ShopList?> GetShopListById(int id);
    Task<ICollection<ShopList>> GetShopListsByUser(string userId);
}

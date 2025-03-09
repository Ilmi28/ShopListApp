using ShopListApp.Models;

namespace ShopListApp.Interfaces.IRepositories
{
    public interface IShopListRepository
    {
        Task AddShopList(ShopList shopList);
        Task<bool> RemoveShopList(int id);
        Task<bool> UpdateShopList(int id, ShopList updatedShopList);
        Task<ShopList?> GetShopListById(int id);
    }
}

using ShopListApp.Commands;
using ShopListApp.ViewModels;

namespace ShopListApp.Interfaces.IServices
{
    public interface IShopListService
    {
        Task CreateShopList(string userId, CreateShopListCommand cmd);
        Task DeleteShopList(int shopListId);
        Task AddProductToShopList(int shopListId, int productId);
        Task RemoveProductFromShopList(int shopListId, int productId);
        Task<ShopListView> GetShopListById(int shopListId);
        Task UpdateShopList(int shopListId, UpdateShopListCommand cmd);
    }
}

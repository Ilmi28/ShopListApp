using ShopListApp.Commands;
using ShopListApp.ViewModels;

namespace ShopListApp.Interfaces.IServices
{
    public interface IShopListService
    {
        Task CreateShopList(CreateShopListCommand cmd);
        Task DeleteShopList(int shopListId);
        Task AddProductToShopList(int shopListId, int productId);
        Task RemoveProductFromShopList(int shopListId, int productId);
        Task<ICollection<ProductView>> GetShopListProducts(int shopListId);
    }
}

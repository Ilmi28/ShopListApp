using ShopListApp.ViewModels;

namespace ShopListApp.Interfaces.IServices
{
    public interface IStoreService
    {
        Task<ICollection<StoreView>> GetStores();
        Task<StoreView> GetStoreById(int id);
    }
}

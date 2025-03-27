using ShopListApp.ViewModels;

namespace ShopListApp.Interfaces.IServices
{
    public interface IStoreService
    {
        Task<ICollection<StoreResponse>> GetStores();
        Task<StoreResponse> GetStoreById(int id);
    }
}

using ShopListApp.Core.Responses;

namespace ShopListApp.Core.Interfaces.IServices
{
    public interface IStoreService
    {
        Task<ICollection<StoreResponse>> GetStores();
        Task<StoreResponse> GetStoreById(int id);
    }
}
